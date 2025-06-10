using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator
{
    private readonly RSA _rsaPrivateKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTokenGenerator(RSA rsaPrivateKey, string issuer, string audience)
    {
        _rsaPrivateKey = rsaPrivateKey;
        _issuer = issuer;
        _audience = audience;
    }

    public string GenerateClientToken(string clientId)
    {
        var credentials = new SigningCredentials(new RsaSecurityKey(_rsaPrivateKey), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, clientId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _audience),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(28), // 4 weeks
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
