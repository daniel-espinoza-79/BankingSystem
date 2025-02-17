using Business.Dtos;
using Business.Wrappers;

namespace Business.Services;

public interface IBankAccountService
{
    Task<Response<Guid>> CreateAccount(CreateBankAccountDto account);
    Task<Response<AccountDto>> GetAccountById(Guid id);
}