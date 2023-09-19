using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Core.Security.Jwt;

public class JwtHelper : IJwtHelper
{

    private readonly ILogger<JwtHelper> _logger;
    public readonly IConfiguration _configuration;
    private readonly JwtSetting _tokenOptions;
    private DateTime _accessTokenExpiration;

    public JwtHelper(ILogger<JwtHelper> logger, IOptions<JwtSetting> tokenOptions)
    {
        _logger = logger;
        _tokenOptions = tokenOptions.Value;

    }
    /// <summary>
    /// claims_options tipi dynamic ör: new {FullName = "SAİD YUNUS", UserName="saidxs2016", RoleName="Admin", AuthID="admin.fc30ce45-b631-4c5d-8cd4-bfa1a33e6e4b.rand_uid", ByConnectionKey="guid", ByUser="guid" AppKey = "guid"}
    /// bir token oluşturulması için gönderilmesi gereken parametereler FullName:Name+Surname, UserName, RoleName, AuthID:Rolename.UserID.RandomGuid, ByConnectionKey: Vekalet girişlerdedeki anahatar, ByUser: vekil Kullanıcı guid'si, AppKey:uygulamanın uidsi
    /// Aynen bu formatta
    /// normal girişlerde ByConnectionKey ve ByUser : null olarak atanır,
    /// 
    /// </summary>
    /// <param name="claims_options">Bu bir dynamic tip </param>
    /// <returns></returns>
    public AccessTokenHelper CreateToken(dynamic claims_options, string expirationDate = null, string addExpirationDate = null)
    {
        var minutes = DateIntervalHelpers.GetInternalAsMinutes(expirationDate);
        var addMinutes = DateIntervalHelpers.GetInternalAsMinutes(addExpirationDate);
        if (minutes == 0)
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration + addMinutes);
        else
            _accessTokenExpiration = DateTime.Now.AddMinutes(minutes + addMinutes);


        AccessTokenHelper accessToken = new();
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var jwt = CreateJwtSecurityToken(claims_options, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            accessToken.Token = token;
            accessToken.Expiration = _accessTokenExpiration;
            accessToken.ReTokenExpiration = _accessTokenExpiration.AddMinutes(30);
            accessToken.ReToken = CreateRefreshToken();
        }
        catch (Exception)
        {
            throw;
        }
        return accessToken;
    }
    private string CreateRefreshToken()
    {
        byte[] number = new byte[32];
        using RandomNumberGenerator random = RandomNumberGenerator.Create();
        random.GetBytes(number);
        return Convert.ToBase64String(number);

    }
    private JwtSecurityToken CreateJwtSecurityToken(dynamic claims_options, SigningCredentials signingCredentials)
    {
        var jwt = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            expires: _accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: SetClaims(claims_options),
            signingCredentials: signingCredentials
            );

        return jwt;
    }
    private IEnumerable<Claim> SetClaims(dynamic claims_options)
    {
        var Name = claims_options.GetType().GetProperty(ClaimHelper.FullName).GetValue(claims_options);
        var RoleName = claims_options.GetType().GetProperty(ClaimHelper.RoleName).GetValue(claims_options);
        var UserName = claims_options.GetType().GetProperty(ClaimHelper.UserName).GetValue(claims_options);
        var AuthID = claims_options.GetType().GetProperty(ClaimHelper.AuthID).GetValue(claims_options);  // (redis || in memory) de tutulan keyin anahtarı            
        var AppKey = claims_options.GetType().GetProperty(ClaimHelper.AppKey).GetValue(claims_options);

        string ByConnectionKey = string.Empty, ByUser = string.Empty;
        try
        {
            ByConnectionKey = claims_options.GetType().GetProperty(ClaimHelper.ByConnectionKey).GetValue(claims_options);
        }
        catch (Exception) { }
        try
        {
            ByUser = claims_options.GetType().GetProperty(ClaimHelper.ByUser).GetValue(claims_options);
        }
        catch (Exception) { }

        List<Claim> claims = new()
        {
            new Claim(ClaimHelper.FullName, Name),
            new Claim(ClaimHelper.UserName, UserName),
            new Claim(ClaimHelper.RoleName, RoleName),
            new Claim(ClaimHelper.AuthID, AuthID),
            new Claim(ClaimHelper.ByConnectionKey, ByConnectionKey),
            new Claim(ClaimHelper.ByUser, ByUser),
            new Claim(ClaimHelper.AppKey, AppKey)
        };
        return claims;
    }



    /// <summary>
    /// (FullName, RoleName, UserName, AuthID, ByConnecyionKey, ByUser, AppKey)
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public (string, string, string, string, string, string, string) ValidateToken(string token)
    {
        try
        {

            var tokenHandler = new JwtSecurityTokenHandler();


            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.SecurityKey)),
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidAudience = _tokenOptions.Audience,
                ValidIssuer = _tokenOptions.Issuer,
                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null && expires > DateTime.UtcNow,
                ClockSkew = TimeSpan.Zero,
            }, out SecurityToken validatedToken);


            var jwtToken = (JwtSecurityToken)validatedToken;
            var fullName = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.FullName)?.Value;
            var roleName = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.RoleName)?.Value;
            var userName = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.UserName)?.Value;
            var authId = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AuthID)?.Value;
            var byConnecyionKey = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.ByConnectionKey)?.Value;
            var byUser = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.ByUser)?.Value;
            var appKey = jwtToken.Claims.FirstOrDefault(i => i.Type == ClaimHelper.AppKey)?.Value;


            return (fullName, roleName, userName, authId, byConnecyionKey, byUser, appKey);
        }
        catch (Exception) { }


        return (null, null, null, null, null, null, null);


    }

}
