using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.Dtos;
using AuthenticationService.Models;
using AuthenticationService.Enums;
using AuthenticationService.Services;

namespace AuthenticationService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
  

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }
    

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.DocumentId,
            FullName = registerDto.FullName,
            Email = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, UserRole.Basic.ToString());
            return Ok(new { message = "User created successfully!" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("/assign-admin")]
    public async Task<IActionResult> AssignAdminRole([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (await _userManager.IsInRoleAsync(user, UserRole.Admin.ToString()))
            return BadRequest(new { message = "User is already an admin" });

        var result = await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
        if (result.Succeeded)
            return Ok(new { message = "Admin role assigned successfully" });

        return BadRequest(result.Errors);
    }

    [HttpPost("/remove-admin")]
    public async Task<IActionResult> RemoveAdminRole([FromBody] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (!await _userManager.IsInRoleAsync(user, UserRole.Admin.ToString()))
            return BadRequest(new { message = "User is not an admin" });

        var result = await _userManager.RemoveFromRoleAsync(user, UserRole.Admin.ToString());
        if (result.Succeeded)
            return Ok(new { message = "Admin role removed successfully" });

        return BadRequest(result.Errors);
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (result.Succeeded)
        {
            var token =   await _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }
}