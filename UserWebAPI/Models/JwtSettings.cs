namespace UserWebAPI.Models
{
    public class JwtSettings
    {
        public string? Key { get; set; }
        public string? AccessTokenValidityInMinute { get; set; }
        public string? RefreshTokenValidityInDay { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
