using Core.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace UI.Extensions.Auth;

public static class AuthRegistration
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:SecurityKey"]));

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = signingKey,
                RequireExpirationTime = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidIssuer = configuration["Jwt:Issuer"],
                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null && expires > DateTime.UtcNow,
                ClockSkew = TimeSpan.Zero,
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var access_token = context.Request.Headers["Authorization"];
                    if (!string.IsNullOrEmpty(access_token))
                        access_token = access_token.ToString().Replace("Bearer ", "");
                    if (string.IsNullOrEmpty(access_token))
                        access_token = context.Request.Cookies["Access-Token"];

                    if (string.IsNullOrEmpty(access_token))
                    {
                        var tmp_token = Convert.ToString(context.Request.Query["access_token"]);
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(tmp_token) && path.StartsWithSegments("/hubs"))
                            access_token = tmp_token;
                    }

                    context.Token = access_token;
                    return Task.CompletedTask;
                },

                OnTokenValidated = context => Task.CompletedTask,
                OnAuthenticationFailed = context => Task.CompletedTask
            };
        });

        return services;
    }
}
