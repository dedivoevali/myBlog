using Common.Dto.Auth;
using Microsoft.IdentityModel.Tokens;
using Service.Abstract.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common;
using Common.Options;
using Microsoft.Extensions.Options;

namespace Service.Auth
{
    public class EncryptionService : IEncryptionService
    {
        private readonly JsonWebTokenOptions _jwtOptions;

        public EncryptionService(IOptions<JsonWebTokenOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public AuthenticateResponse GenerateAccessToken(int userId, string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            Claim[] claims =
            [
                new(TokenClaimNames.Id, userId.ToString()),
                new(TokenClaimNames.Username, username)
            ];

            var notBefore = DateTime.UtcNow;
            var expires = notBefore.AddMinutes(_jwtOptions.ValidityTimeMinutes);
            var tokenObject = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return new AuthenticateResponse
            {
                Id = userId,
                Username = username,
                Token = token,
                AuthorizationExpirationDate = expires
            };
        }

        public string EncryptPassword(string phrase)
        {
            var byteArrayPhrase = Encoding.UTF8.GetBytes(phrase);
            using var algorithm = SHA256.Create();
            var hashBytes = algorithm.ComputeHash(byteArrayPhrase);
            var hash = BitConverter.ToString(hashBytes).ToLower().Replace("-","");
            return hash;
        }
    }
}