using Microsoft.IdentityModel.Tokens;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Pardis.Application._Shared.JWT
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Email ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // افزودن نقش‌ها به کلیم‌ها
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ✅ استفاده از تنظیمات ExpiryInHours از appsettings
            var expiryHours = _config.GetValue<int>("JwtSettings:ExpiryInHours", 24);
            var expiry = DateTime.UtcNow.AddHours(expiryHours);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                NotBefore = DateTime.UtcNow, // Token valid from now
                IssuedAt = DateTime.UtcNow,  // Token issued at
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"] ?? "PardisAcademy",
                Audience = _config["JwtSettings:Audience"] ?? "PardisAcademyUsers"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);
            
            // ✅ Debug log برای بررسی token
            Console.WriteLine($"Generated JWT Token for user {user.Email}: {tokenString.Substring(0, Math.Min(50, tokenString.Length))}...");
            Console.WriteLine($"Token expires at: {expiry:yyyy-MM-dd HH:mm:ss} UTC");

            return tokenString;
        }
    }
}
