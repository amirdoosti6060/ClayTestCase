namespace UserWebAPI.Models
{
    public class JwtSettings
    {
        public string? JwtSettings_Key { get; set; }
        public string? JwtSettings_AccessTokenValidityInMinute { get; set; }
        public string? JwtSettings_Issuer { get; set; }
        public string? JwtSettings_Audience { get; set; }
    }
}
