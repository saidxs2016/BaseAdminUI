using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UI.Middlewares;

public class JwtTokenHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    public JwtTokenHandlerMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try 
        {
            var token = httpContext.Request.Cookies["Access-Token"].Split(" ").Last();
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                // ============ 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:SecurityKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
                LifetimeValidator = (notBefore, expires, securityToken, validateParameters) => expires != null && expires > DateTime.Now,
                ClockSkew = TimeSpan.Zero,
            }, out SecurityToken validatedToken);


            var jwtToken = (JwtSecurityToken)validatedToken;
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(jwtToken.Claims, "JwtBearer");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            httpContext.User = claimsPrincipal;
        }
        catch (Exception) { }



        await _next.Invoke(httpContext);


    }
}

public static class JwtTokenMiddleware
{
    public static WebApplication UseJwtMiddleware(this WebApplication app)
    {
        app.UseMiddleware<JwtTokenHandlerMiddleware>();
        return app;
    }
}
