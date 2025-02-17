using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace OAuthServer;

public class DevKey
{
    public RSA RsaKey { get;}
    public RsaSecurityKey RsaSecurityKey  => new RsaSecurityKey(RsaKey);
    public DevKey(IWebHostEnvironment env)
    {
        RsaKey = RSA.Create();
        var path = Path.Combine(env.ContentRootPath, "crypto_key");
        if (File.Exists(path))
        {
            var rsaKey = RSA.Create();
            rsaKey.ImportRSAPrivateKey(File.ReadAllBytes(path), out _);
        }
        else
        {
            var privateKey =RsaKey.ExportRSAPrivateKey();
            File.WriteAllBytes(path, privateKey);
        }
    }
}