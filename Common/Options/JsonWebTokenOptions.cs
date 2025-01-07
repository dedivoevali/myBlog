namespace Common.Options
{
    public class JsonWebTokenOptions : BaseApplicationOptions
    {
        public new static string Config => "Jwt";
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenValidityTimeMinutes { get; set; }
        public int RefreshTokenValidityTimeHours { get; set; }
        public TimeSpan AccessTokenValidityTime => TimeSpan.FromMinutes(AccessTokenValidityTimeMinutes);
        public TimeSpan RefreshTokenValidityTime => TimeSpan.FromHours(RefreshTokenValidityTimeHours);
    }
}