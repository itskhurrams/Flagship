using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Flagship.Infrastructure.Extension.Security {
    public class Authentication {
        public static void AddAuthenication(IServiceCollection services, IConfiguration configuration) {
            var configuredSecretKey = configuration["TokenAuthentication:SecretKey"];
            var secretKey = !string.IsNullOrWhiteSpace(configuredSecretKey)
                ? configuredSecretKey
                : throw new InvalidOperationException("Missing required configuration value 'TokenAuthentication:SecretKey'. Set it via User Secrets or an environment variable.");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var tokenValidationParams = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = configuration["TokenAuthentication:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["TokenAuthentication:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            services.AddAuthentication(authentication => {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtValidator => {
                jwtValidator.TokenValidationParameters = tokenValidationParams;
            });
        }
    }
}
