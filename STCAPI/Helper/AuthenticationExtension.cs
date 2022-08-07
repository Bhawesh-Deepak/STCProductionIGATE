using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace STCAPI.Helper
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthenticationToken(this IServiceCollection services, IConfiguration configuration)
        {
            //var sceret = configuration.GetSection("").GetSection("").Value;
            //var key = Encoding.ASCII.GetBytes(sceret);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(x =>
            //{
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidIssuer = "localhost",
            //        ValidAudience = "localhost"
            //    };
            //});

            return services;

        }
    }
}
