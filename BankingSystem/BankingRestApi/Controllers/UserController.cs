using Business.Dtos;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingRestApi.Controllers
{    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ILogger<UserController> logger, IUserService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int threshold = 1, [FromQuery] int limit = 10)
        {
            var response = await userService.GetAll(threshold, limit);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await userService.GetById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await userService.Create(createUserDto);
            return CreatedAtAction(nameof(GetById), new { id = response.Data }, response.Data);
        }
    }
}
