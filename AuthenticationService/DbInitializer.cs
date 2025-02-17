using AuthenticationService.Enums;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService;


public class DbInitializer
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public DbInitializer(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task Initialize()
    {
        await CreateRoles();
        
        await CreateAdminUser();
    }

    private async Task CreateRoles()
    {
        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        {
            string roleName = role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private async Task CreateAdminUser()
    {
        var adminEmail = _configuration["DefaultAdminUser:Email"];
        var adminPassword = _configuration["DefaultAdminUser:Password"];
        var adminDocumentId = _configuration["DefaultAdminUser:DocumentId"];
        var adminFullName = _configuration["DefaultAdminUser:FullName"];

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = adminDocumentId,
                Email = adminEmail,
                FullName = adminFullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRole.Basic.ToString());
                await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
            }
        }
    }
}