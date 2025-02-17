using OAuthServer;
using OAuthServer.Endpoints;
using OAuthServer.Endpoints.Oauth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.LoginPath = "/login";
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<DevKey>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/login", (HttpContext ctx) =>
{
    
    
    return Results.Redirect($"http://localhost:7072/signin"+ctx.Request.Query["returnUrl"]);
});
app.MapPost("/login", Login.Handler);
app.MapGet("/oauth/authorize", AuthorizationEndpoint.Handle).RequireAuthorization();
app.MapPost("/oauth/token", TokenEndpoint.Handle);



app.UseHttpsRedirection();

app.Run();