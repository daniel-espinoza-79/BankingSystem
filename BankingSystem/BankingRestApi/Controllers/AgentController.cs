using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/agents")]
public class AgentController(IAgentService agentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAtms([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await agentService.GetAllAtms(page, limit);
        return response.Succeded ? Ok(response.Data) : BadRequest(response.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAtmById(Guid id)
    {
        var response = await agentService.GetAtmById(id);
        return response.Succeded ? Ok(response.Data) : NotFound(response.Message);
    }
}