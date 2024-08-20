using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CubeEnergy.Utilities
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _refreshTokenSecretKey;
        private readonly string _refreshTokenSecret;

        public JwtService(string secretKey, string issuer, string audience, string refreshTokenSecret)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
            _refreshTokenSecret = refreshTokenSecret;
            
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_refreshTokenSecret);

                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Optional: Adjust if needed
                }, out SecurityToken validatedToken);

                // If we get here, the token is valid
                return true;
            }
            catch
            {
                // Token validation failed
                return false;
            }
        }

        public string GenerateOTP()
        {
            var random = new Random();
            var otp = random.Next(100000, 999999); // Generates a 6-digit OTP
            return otp.ToString();
        }

        public string GenerateToken(string username)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty when generating a refresh token.");
            }

            var key = Encoding.UTF8.GetBytes(username);
            using (var hmac = new HMACSHA256(key))
            {
                var refreshToken = Convert.ToBase64String(hmac.ComputeHash(Guid.NewGuid().ToByteArray()));
                return refreshToken;
            }
        }

    }

}
