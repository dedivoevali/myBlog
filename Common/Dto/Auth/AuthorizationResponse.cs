namespace Common.Dto.Auth
{
    public record AuthorizationResponse
    {
        public required int UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}