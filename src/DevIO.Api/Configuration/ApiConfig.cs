using DevIO.Api.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {

            services.AddControllers();

            #region Controle de versão
            services.AddApiVersioning(options => 
            {
                //options.AssumeDefaultVersionWhenUnspecified = true;
                //options.DefaultApiVersion = new ApiVersion(1, 0);
                //options.ReportApiVersions = true; // passa informação no header da versão
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer( options => 
            {
                //options.GroupNameFormat = "'v'VVV";// majior, minor, path
                //options.SubstituteApiVersionInUrl = true; // caso não passe uma versão especifica na URL a versão default é assumida
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            #endregion

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            #region Cors
            // cors diminui as restrições de acesso
            services.AddCors(options =>
            {
                // define a default policy if necessary to be used 
                //options.AddDefaultPolicy(
                //    builder => builder
                //        .AllowAnyOrigin()
                //        .AllowAnyMethod()
                //        .AllowAnyHeader()
                //        .AllowCredentials());

                options.AddPolicy("Development",
                    policyBuilder => policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                options.AddPolicy("Production",
                    policyBuilder => policyBuilder
                        .WithMethods("GET") // permite varios http methods
                        .WithOrigins("http://localhost") // permite varios dominios
                        .SetIsOriginAllowedToAllowWildcardSubdomains() // allow subdomains to Origins informed
                        //.WithHeaders(HeaderNames.ContentType, "x-custom-header")
                        .AllowAnyHeader());
            });
            #endregion

            return services;
        }

        public static IApplicationBuilder UseApiConfig(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            // Configure the HTTP request pipeline.
            if (environment.IsDevelopment())
            {
                app.UseCors("Development");
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }
            else
            {
                app.UseCors("Production"); // this don't work it's just to be a sample
                app.UseHsts(); // recurso de segurança para certificado digital
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection(); // redireciona para HTTPS

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                #region HealthChecks application
                /*
                 * configuração de utilização dos HealthChecks
                 * 
                 * parametro: {caminho base da api}/api/hc
                 * serve para verificar a saúde da API sem interface visual
                 * 
                 * parametro: {caminho base da api}/api/hc-ui
                 * serve para verificar a saúde da API com interface visual
                 */
                //app.UseHealthChecks("/api/hc");
                //app.UseHealthChecksUI(options =>
                //{
                //    options.UIPath = "/api/hc-ui";
                //});
                endpoints.MapHealthChecks("/api/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI(options =>
                {
                    options.UIPath = "/api/hc-ui";
                    options.ResourcesPath = "/api/hc-ui-resources";

                    options.UseRelativeApiPath = false;
                    options.UseRelativeResourcesPath = false;
                    options.UseRelativeWebhookPath = false;
                });
                #endregion
            });

            return app;
        }
    }
}
