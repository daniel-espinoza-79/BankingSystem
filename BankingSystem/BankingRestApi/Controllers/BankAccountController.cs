using Business.Dtos;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController(ILogger<UserController> logger, IBankAccountService bankAccountService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;

        [HttpPost()]
        public async Task<IActionResult> CreateSavingAccount([FromBody] CreateBankAccountDto createBankAccountDto)
        {
            var response = await bankAccountService.CreateAccount(createBankAccountDto);
            return Ok(response);
        }
        
        
        [HttpGet("/{accountNumber}")]
        public async Task<IActionResult> GetBankAccountById(Guid accountNumber)
        {
            var response = await bankAccountService.GetAccountById(accountNumber);
            return Ok(response);
        }
    }
}