using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace OAuthServer.Endpoints.Oauth;

public static class TokenEndpoint
{

    public static async Task<IResult> Handle(HttpRequest request,
        DevKey keys,
        IDataProtectionProvider dataProtectionProvider)
    {
        var bodyBytes = await request.BodyReader.ReadAsync();
        var bodyContent = Encoding.UTF8.GetString(bodyBytes.Buffer);


        string grantType = "", code = "", redirectUri = "", codeVerifier = "";
        foreach (var part in bodyContent.Split("&"))
        {
            var subParts= part.Split("=");
            var key = subParts[0];
            var value = subParts[1];
            if (key == "grant_type") grantType=value;
            else if (key == "code") code = value;
            else if (key == "redirect_uri") redirectUri = value;
            else if (key == "code_verifier") codeVerifier = value;
        }

        var protector = dataProtectionProvider.CreateProtector("OAuth");
        var codeString = protector.Unprotect(code);
        var authCode = JsonSerializer.Deserialize<AuthCode>(codeString);

        if (!ValidateCodeVerifier(authCode, codeVerifier))
        {
            return Results.BadRequest("Invalid code");
        }
        var handler = new JsonWebTokenHandler();
        
        return Results.Ok(new
        {
            access_token = handler.CreateToken(new SecurityTokenDescriptor()
            {
                Claims=new Dictionary<string, object>()
                {
                    
                    [JwtRegisteredClaimNames.Sub] = Guid.NewGuid().ToString(),
                    ["custom"]="foo"
                },
                Expires = DateTime.Now.AddMinutes(15),
                TokenType = "Bearer",   
                SigningCredentials = new SigningCredentials(keys.RsaSecurityKey, SecurityAlgorithms.RsaSha256),
            }),
            token_type = "Bearer",
        });
    }


    private static  bool ValidateCodeVerifier(
        AuthCode authCode  ,string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var codeChallenge = Base64UrlTextEncoder.Encode(sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier)));
        return authCode.CodeChallenge == codeChallenge;
    }
    
    

    
}