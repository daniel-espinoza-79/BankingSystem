using Business.Dtos.Transactions;
using Business.Wrappers;
using Domain.Entities.Concretes;

namespace Business.Services;

public interface ITransactionService
{
    Task<Response<TransactionResponseDto>> ProcessDepositAsync(TransactionRequestDto depositDto);
    
    Task<Response<TransactionResponseDto>> ProcessWithdrawAsync(WithdrawRequestDto withdrawDto);
    
    Task<Response<TransactionResponseDto>> ProcessTransferAsync(TransactionRequestDto transferDto);
    
    Task<Response<TransactionResponseDto>> GetTransactionByIdAsync(Guid id);
    
    Task<Response<IEnumerable<TransactionResponseDto>>> GetAccountTransactionHistoryAsync(
        Guid accountId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        TransactionType? type = null);
    
}
