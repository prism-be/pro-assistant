using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Prism.ProAssistant.Business.Exceptions;
using Prism.ProAssistant.Business.Users;

namespace Prism.ProAssistant.Business.Security;

public static class TokenGenerator
{
    public static string GenerateAccessToken(string privateKey, User user)
    {
        var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Identifier.Generate().ToString()),
            new(ClaimsNames.Name, user.Name),
            new(ClaimsNames.UserId, user.Id)
        };

        return GenerateToken(privateKey, claims, DateTime.Now.AddMinutes(30), false);
    }

    public static string GenerateRefreshToken(string privateKey, User user)
    {
        var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Identifier.Generate().ToString()),
            new(ClaimsNames.UserId, user.Id)
        };

        return GenerateToken(privateKey, claims, DateTime.Now.AddDays(30), true);
    }

    public static ClaimsPrincipal? ValidateToken(string publicKey, string token, ILogger logger, bool isRefreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new MissingConfigurationException("The configuration of jwtConfiguration.PublicKey is empty", "JWT_PUBLIC_KEY");
            }

            var publicKeyBytes = Convert.FromBase64String(publicKey);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtConfiguration.Issuer,
                ValidAudience = JwtConfiguration.Audience + (isRefreshToken ? "-refresh" : string.Empty),
                IssuerSigningKey = new RsaSecurityKey(rsa),
                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                },
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch (MissingConfigurationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error occured when validating bearer");
            return null;
        }
    }

    private static string GenerateToken(string privateKey, IEnumerable<Claim> claims, DateTime expiration, bool isRefreshToken)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKey);

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        var now = DateTime.Now;

        var jwt = new JwtSecurityToken(
            audience: JwtConfiguration.Audience + (isRefreshToken ? "-refresh" : string.Empty),
            issuer: JwtConfiguration.Issuer,
            claims: claims,
            notBefore: now,
            expires: expiration,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}