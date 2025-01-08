using Common.Dto.Auth;

namespace Service.Abstract.Auth
{
    public interface IEncryptionService
    {
        public string GenerateAccessToken(int userId, string username);
        public string EncryptPassword(string phrase);
        public RefreshToken GenerateRefreshToken();
    }
}