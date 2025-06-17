using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SoepkipAPI.Interfaces;

namespace SoepkipAPI.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _privateKeyMonitoring;
    private readonly SymmetricSecurityKey _privateKeySensoring;
    private readonly string _issuer;
    private readonly string _audience;
    
    public JwtTokenGenerator(IConfiguration config)
    {
        _config = config;
        var monitoringKey = Environment.GetEnvironmentVariable("JWT_KEY_MONITORING") ?? config["Jwt:MonitoringKey"] 
            ?? "defaultkeydefaultkeydefaultkeydefaultkey";
        _privateKeyMonitoring = new(Encoding.UTF8.GetBytes(monitoringKey));
        
        var sensoringKey = Environment.GetEnvironmentVariable("JWT_KEY_SENSORING") ?? config["Jwt:SensoringKey"] 
            ?? "defaultkeydefaultkeydefaultkeydefaultkey";
        _privateKeySensoring = new(Encoding.UTF8.GetBytes(sensoringKey));
        
        _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? config["Jwt:Issuer"] ?? "default";
        _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? config["Jwt:Audience"] ?? "default";
    }
    

    public string GenerateClientToken(string key)
    {
        //Default key does not work
        SigningCredentials credentials;
        
        //Monitoring key
        if (key == _config["Jwt:MonitoringKey"])
        {
            credentials = new SigningCredentials(_privateKeyMonitoring, SecurityAlgorithms.HmacSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
        }      
        
        //Sensoring key
        else if (key == _config["Jwt:SensoringKey"])
        {
            credentials = new SigningCredentials(_privateKeyMonitoring, SecurityAlgorithms.HmacSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
        }
        else
        {
            throw new SecurityException("Invalid signing key");
        }
        

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "Soepkip"),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(4), //4 hours
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}