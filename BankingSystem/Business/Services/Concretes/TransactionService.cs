using AutoMapper;
using Business.Dtos.Transactions;
using Business.Wrappers;
using DataAccess.Repositories.Interfaces;
using Domain.Entities.Concretes;

namespace Business.Services;

public class TransactionService : ITransactionService
{
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IRepository<Transactions, Guid> _transactionRepository;
    private readonly IRepository<BankAccount, Guid> _bankAccountRepository;
    private readonly IRepository<Agent, Guid> _agentRepository;
    private readonly IMapper _mapper;

    public TransactionService(
        IRepository<User, Guid> userRepository,
        IRepository<Transactions, Guid> transactionRepository,
        IRepository<BankAccount, Guid> bankAccountRepository,
        IRepository<Agent, Guid> agentRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _bankAccountRepository = bankAccountRepository;
        _agentRepository = agentRepository;
        _mapper = mapper;
    }

    public async Task<Response<TransactionResponseDto>> ProcessDepositAsync(TransactionRequestDto depositDto)
    {
        // Validate account and agent
        var destinationAccount = await ValidateAccount(depositDto.AccountDestinationId);
        var agent = await ValidateAgent(depositDto.AgentId);
        
        var transaction = new Transactions
        {
            AccountDestinationId = depositDto.AccountDestinationId,
            AgentId = depositDto.AgentId,
            Amount = depositDto.Amount,
            TransactionType = TransactionType.Deposit,
            Status = TransactionStatus.Processing,
            TransactionDate = DateTime.UtcNow,
            Description = depositDto.Description,
            Agent = agent
        };

        await _transactionRepository.CreateAsync(transaction);

        try
        {
            // Update  bank account balance
            destinationAccount.Balance += (int)depositDto.Amount;
            await _bankAccountRepository.UpdateAsync(destinationAccount.Id, destinationAccount);

            // Update transaction status
            transaction.Status = TransactionStatus.Completed;
            await _transactionRepository.UpdateAsync(transaction.Id, transaction);


            return Response<TransactionResponseDto>.Success(_mapper.Map<TransactionResponseDto>(transaction));
        }
        catch (Exception)
        {
            transaction.Status = TransactionStatus.Failed;
            await _transactionRepository.UpdateAsync(transaction.Id, transaction);
            throw;
        }
    }

    public async Task<Response<TransactionResponseDto>> ProcessWithdrawAsync(WithdrawRequestDto withdrawDto)
    {
        try
        {
            // Validate account and agent
            var account = await ValidateAccount(withdrawDto.AccountId);
            var agent = await ValidateAgent(withdrawDto.AgentId);

            // Validate sufficient funds
            if (account.Balance < withdrawDto.Amount)
                return Response<TransactionResponseDto>.Failure("Insufficient funds");

            var transaction = new Transactions
            {
                AccountOriginId = withdrawDto.AccountId,
                AgentId = withdrawDto.AgentId,
                Amount = withdrawDto.Amount,
                TransactionType = TransactionType.Withdraw,
                Status = TransactionStatus.Processing,
                TransactionDate = DateTime.UtcNow,
                Description = withdrawDto.Description,
                Agent = agent
            };

            await _transactionRepository.CreateAsync(transaction);

            try
            {
                // Update account balance
                account.Balance -= (int)withdrawDto.Amount;
                await _bankAccountRepository.UpdateAsync(account.Id, account);

                // Update ATM balance if applicable
                if (agent is ATM atm)
                {
                    atm.CurrentBalance -= (int)withdrawDto.Amount;
                    await _agentRepository.UpdateAsync(atm.Id, atm);
                }

                transaction.Status = TransactionStatus.Completed;
                await _transactionRepository.UpdateAsync(transaction.Id, transaction);

                return Response<TransactionResponseDto>.Success(_mapper.Map<Transactions,TransactionResponseDto>(transaction));
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                await _transactionRepository.UpdateAsync(transaction.Id, transaction);
                return Response<TransactionResponseDto>.Failure("Failed to process withdrawal");
            }
        }
        catch (KeyNotFoundException ex)
        {
            return Response<TransactionResponseDto>.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Response<TransactionResponseDto>.Failure("An unexpected error occurred");
        }
    }

    public async Task<Response<TransactionResponseDto>> ProcessTransferAsync(TransactionRequestDto transferDto)
    {
        try
        {
            // Validate accounts and agent
            var originAccount = await ValidateAccount(transferDto.AccountOriginId);
            var destinationAccount = await ValidateAccount(transferDto.AccountDestinationId);
            var agent = await ValidateAgent(transferDto.AgentId);

            // Validate sufficient funds
            if (originAccount.Balance < transferDto.Amount)
                return Response<TransactionResponseDto>.Failure("Insufficient funds");

            var transaction = new Transactions
            {
                AccountOriginId = transferDto.AccountOriginId,
                AccountDestinationId = transferDto.AccountDestinationId,
                AgentId = transferDto.AgentId,
                Amount = transferDto.Amount,
                TransactionType = TransactionType.Transfer,
                Status = TransactionStatus.Processing,
                TransactionDate = DateTime.UtcNow,
                Description = transferDto.Description,
                Agent = agent
            };

            await _transactionRepository.CreateAsync(transaction);

            try
            {
                // Update balances
                originAccount.Balance -= (int)transferDto.Amount;
                destinationAccount.Balance += (int)transferDto.Amount;

                await _bankAccountRepository.UpdateAsync(originAccount.Id, originAccount);
                await _bankAccountRepository.UpdateAsync(destinationAccount.Id, destinationAccount);

                transaction.Status = TransactionStatus.Completed;
                await _transactionRepository.UpdateAsync(transaction.Id, transaction);

                return Response<TransactionResponseDto>.Success(_mapper.Map<TransactionResponseDto>(transaction));
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                await _transactionRepository.UpdateAsync(transaction.Id, transaction);
                return Response<TransactionResponseDto>.Failure("Failed to process transfer");
            }
        }
        catch (KeyNotFoundException ex)
        {
            return Response<TransactionResponseDto>.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Response<TransactionResponseDto>.Failure("An unexpected error occurred");
        }
    }

    public async Task<Response<TransactionResponseDto>> GetTransactionByIdAsync(Guid id)
    {
        try
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
                return Response<TransactionResponseDto>.Failure("Transaction not found");

            return Response<TransactionResponseDto>.Success(_mapper.Map<TransactionResponseDto>(transaction));
        }
        catch (Exception)
        {
            return Response<TransactionResponseDto>.Failure("An unexpected error occurred");
        }
    }

    public async Task<Response<IEnumerable<TransactionResponseDto>>> GetAccountTransactionHistoryAsync(
        Guid accountId, 
        DateTime? startDate = null, 
        DateTime? endDate = null,
        TransactionType? type = null)
    {
        try
        {
            var transactions = await _transactionRepository.FindAsync(t => 
                (t.AccountOriginId == accountId || t.AccountDestinationId == accountId) &&
                (!startDate.HasValue || t.TransactionDate >= startDate) &&
                (!endDate.HasValue || t.TransactionDate <= endDate) &&
                (!type.HasValue || t.TransactionType == type));

            return Response<IEnumerable<TransactionResponseDto>>.Success(
                _mapper.Map<IEnumerable<TransactionResponseDto>>(transactions));
        }
        catch (Exception)
        {
            return Response<IEnumerable<TransactionResponseDto>>.Failure("Failed to retrieve transaction history");
        }
    }
    
    private async Task<BankAccount> ValidateAccount(Guid accountId)
    {
        var account = await _bankAccountRepository.GetByIdAsync(accountId);
        if (account == null)
            throw new KeyNotFoundException("Account not found");
        return account;
    }

    private async Task<Agent> ValidateAgent(Guid agentId)
    {
        var agent = await _agentRepository.GetByIdAsync(agentId);
        if (agent == null)
            throw new KeyNotFoundException("Agent not found");
        return agent;
    }
}