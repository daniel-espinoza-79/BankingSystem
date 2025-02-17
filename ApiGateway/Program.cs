using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using ApiGateway.ServerConfigurations;
using DotNetEnv;
using Ocelot.Middleware;

Env.Load("../.env");


ServicePointManager.ServerCertificateValidationCallback = 
    delegate (
        object sender, 
        X509Certificate certificate, 
        X509Chain chain, 
        SslPolicyErrors sslPolicyErrors)
    {
        return true; // Accept all certificates
    };

var builder = WebApplication.CreateBuilder(args);


string hostUrl = builder.Configuration["WebHostUrl"] ?? "http://localhost:5214";


builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder => builder.WithOrigins(hostUrl)
                              .AllowAnyMethod()
                              .AllowAnyHeader());
    });

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});
builder.Services.ConfigureOcelot(builder.Configuration);
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfig();
}


app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();