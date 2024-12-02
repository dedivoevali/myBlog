using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using API.Middlewares.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API.Extensions.Auth
{
    public static class JwtBearerConfiguration
    {
        public static void LoadConfigurationForJwtBearer(this AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration)
        {
            authenticationBuilder.AddJwtBearer(
                options =>
                {
                    var securityKeyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new ArgumentNullException());
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytes)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            var error = new ErrorDetails(
                                statusCode: (int)HttpStatusCode.Unauthorized,
                                message: "Authentication failed!",
                                stackTrace: string.Empty
                            );

                            var json = JsonSerializer.SerializeToUtf8Bytes(error);
                            await context.Response.BodyWriter.WriteAsync(json);
                        }
                    };
                });
        }
    }
}