using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Configurations;

public class AuthConfiguration
{
    public const string Issuer = "MyAuthServer";
    public const string Audience = "MyAuthClient";
    public const int Lifetime = 120;
    const string Key = "mysupersecret_secretkey!123srfdf";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}