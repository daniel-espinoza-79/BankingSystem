using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AuthenticationService;
using AuthenticationService.Context;
using AuthenticationService.Models;
using AuthenticationService.Services;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
ServicePointManager.ServerCertificateValidationCallback = 
    delegate (
        object sender, 
        X509Certificate certificate, 
        X509Chain chain, 
        SslPolicyErrors sslPolicyErrors)
    {
        return true; // Accept all certificates
    };


Env.Load("../.env");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        b => b.WithOrigins("https://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitializer = services.GetRequiredService<DbInitializer>();
    await dbInitializer.Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI();
    
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();