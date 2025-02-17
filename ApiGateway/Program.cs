using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using ApiGateway.ServerConfigurations;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;
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


builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("custom", o =>
    {
        o.SignInScheme= "cookie";
        o.ClientId="clientXID";
        o.ClientSecret="secretXID";
        
        o.AuthorizationEndpoint="https://localhost:5001/oauth/authorize";
        o.TokenEndpoint="https://localhost:5001/oauth/token";
        o.CallbackPath = "/oauth/custom-cb";
        
        o.UsePkce=true;
        o.ClaimActions.MapJsonKey("sub","sub");
        o.ClaimActions.MapJsonKey("custom 33","custom");
        o.Events.OnCreatingTicket = async ctx =>
        {
            var payloadBase64 = ctx.AccessToken.Split('.')[1];
            var payloadJson = Base64UrlTextEncoder.Decode(payloadBase64);
            var payload = JsonDocument.Parse(payloadJson);
            ctx.RunClaimActions(payload.RootElement);
        };
        o.BackchannelHttpHandler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, errors) => true
            }
        };
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfig();
}


app.UseHttpsRedirection();

app.MapGet("/login", () =>
{
    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = "http://localhost:7072",    
        },
        authenticationSchemes:new List<string>(){"custom"});
});




app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();