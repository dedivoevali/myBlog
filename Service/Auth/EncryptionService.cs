using Microsoft.IdentityModel.Tokens;
using Service.Abstract.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common;
using Common.Dto.Auth;
using Common.Options;
using Common.Validation;
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

        public string GenerateAccessToken(int userId, string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            Claim[] claims =
            [
                new(TokenClaimNames.Id, userId.ToString()),
                new(TokenClaimNames.Username, username)
            ];

            var notBefore = DateTime.UtcNow;
            var expires = notBefore.Add(_jwtOptions.AccessTokenValidityTime);
            var tokenObject = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                notBefore: notBefore,
                expires: expires,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        public string EncryptPassword(string phrase)
        {
            var byteArrayPhrase = Encoding.UTF8.GetBytes(phrase);
            using var algorithm = SHA256.Create();
            var hashBytes = algorithm.ComputeHash(byteArrayPhrase);
            var hash = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");
            return hash;
        }

        public RefreshToken GenerateRefreshToken()
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const int tokenLength = EntityConfigurationConstants.RefreshTokenLength;

            using var rng = RandomNumberGenerator.Create();
            var sb = new StringBuilder(tokenLength);
            var randomNumber = new byte[4];
            for (var i = 0; i < tokenLength; i++)
            {
                rng.GetBytes(randomNumber);
                var randomIndexInAlphabet = BitConverter.ToInt32(randomNumber, 0);
                if (randomIndexInAlphabet < 0)
                {
                    randomIndexInAlphabet *= -1;
                }

                randomIndexInAlphabet %= alphabet.Length;

                sb.Append(alphabet[randomIndexInAlphabet]);
            }

            return new RefreshToken
            {
                Value = sb.ToString(),
                ExpiresAt = DateTime.UtcNow.Add(_jwtOptions.RefreshTokenValidityTime)
            };
        }
    }
}