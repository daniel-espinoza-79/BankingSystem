
using Business.Dtos.Transactions;
using Business.Services;
using Business.Wrappers;
using Domain.Entities.Concretes;
using Microsoft.AspNetCore.Mvc;

namespace BankingRestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("deposit")]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessDeposit([FromBody] TransactionRequestDto depositDto)
    {
        var response = await _transactionService.ProcessDepositAsync(depositDto);
        return response.Succeded ? Ok(response) : BadRequest(response);
    }

    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessWithdraw([FromBody] WithdrawRequestDto withdrawDto)
    {
        var response = await _transactionService.ProcessWithdrawAsync(withdrawDto);
        return response.Succeded ? Ok(response) : BadRequest(response);
    }

    [HttpPost("transfer")]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ProcessTransfer([FromBody] TransactionRequestDto transferDto)
    {
        var response = await _transactionService.ProcessTransferAsync(transferDto);
        return response.Succeded ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<TransactionResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var response = await _transactionService.GetTransactionByIdAsync(id);
        return response.Succeded ? Ok(response) : NotFound(response);
    }

    [HttpGet("account/{accountId:guid}")]
    [ProducesResponseType(typeof(Response<IEnumerable<TransactionResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<IEnumerable<TransactionResponseDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Response<IEnumerable<TransactionResponseDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAccountTransactionHistory(
        Guid accountId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] TransactionType? type = null)
    {
        var response = await _transactionService.GetAccountTransactionHistoryAsync(accountId, startDate, endDate, type);
        return response.Succeded ? Ok(response) : BadRequest(response);
    }
}