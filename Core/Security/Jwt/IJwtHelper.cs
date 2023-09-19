namespace Core.Security.Jwt;

public interface IJwtHelper
{
    AccessTokenHelper CreateToken(dynamic claims_options, string expirationDate = null, string addExpirationDate = null);


    (string, string, string, string, string, string, string) ValidateToken(string token);
}
