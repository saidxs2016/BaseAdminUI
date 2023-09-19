namespace Core.DTO.Options;

public class JwtSetting
{
    public string? SecurityKey { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Subject { get; set; }
    public int AccessTokenExpiration { get; set; }
}
