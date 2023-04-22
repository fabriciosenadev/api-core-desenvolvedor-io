using DevIO.Api.Data;
using DevIO.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DevIO.Api.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddDefaultTokenProviders();

            #region JWT
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication( x => 
            { 
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // gera um token
                x.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme; // valida o token usado na autenticação
            }).AddJwtBearer(x => 
            { 
                x.RequireHttpsMetadata = true; // valida a requisição http sendo apenas https como valida neste caso
                x.SaveToken = true; // mantem o token salvo
                x.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateIssuerSigningKey = true, // valida o token pelo emissor com base no nome do issuer e na chave 
                    IssuerSigningKey = new SymmetricSecurityKey(key), // fornece a chave do emissor do token para o metodo
                    ValidateIssuer = true, // valida apenas o emissor pelo nome 
                    ValidateAudience = true, // valida a origem via url
                    ValidAudience = appSettings.ValidoEm, // fornece a url de origem permitida
                    ValidIssuer = appSettings.Emissor, // fornece o nome do emissor do token
                };
            });
            #endregion

            return services;
        }
    }
}
