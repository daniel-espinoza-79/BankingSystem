using AutoMapper;
using Business.Dtos;
using Business.Wrappers;
using DataAccess.Repositories.Interfaces;
using Domain.Entities.Concretes;

namespace Business.Services;

public class BankAccountService(IRepository<BankAccount, Guid> bankRepository, IRepository<User , Guid> userRepository, IMapper mapper): IBankAccountService
{
    public async Task<Response<Guid>> CreateAccount(CreateBankAccountDto newBankAccount)
    {
        var user = await userRepository.GetByIdAsync(newBankAccount.UserId);
        if (user == null)
        {
            return Response<Guid>.Failure("User not found");
        }

        var savingAccount = new BankAccount()
        {
            Id = Guid.NewGuid(),
            UserId = newBankAccount.UserId,
            User = user,
            Balance = (int)newBankAccount.Balance,
            AccountType = AccountType.SavingAccount,
        };
        
        await bankRepository.CreateAsync(savingAccount);
        user.BankAccounts.Add(savingAccount);
        await userRepository.UpdateAsync(newBankAccount.UserId, user);
        return Response<Guid>.Success(savingAccount.Id);
    }
    
    public async Task<Response<AccountDto>> GetAccountById(Guid id)
    {
        var account = await bankRepository.GetByIdAsync(id);
    
        if (account == null)
        {
            return Response<AccountDto>.Failure("Account not found");
        }

        return Response<AccountDto>.Success(
            new AccountDto()
            {
                Id = account.Id,
                Balance = account.Balance,
                AccountHolder = account.UserId.ToString(),
            }
            );
    }

}