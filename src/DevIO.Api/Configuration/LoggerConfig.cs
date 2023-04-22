using DevIO.Api.Extensions;
using Elmah.Io.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        /**
         * classe de logging da aplicação com monitoramento 
         * 
         * pacote instalado: Elmah.Io.AspNetCore
         * site com doc: https://elmah.io/
         * 
         * integrar o HealthChecks com Elmah
         * pacote: Elmah.Io.AspNetCore.HealthChecks
         * 
         */

        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            /**
             * API KEY: chave disponibilizada no painel
             * LogId: chave disponibilizada no painel
             */

            //services.AddElmahIo(
            //    o =>
            //    {
            //        // dados de exemplo, os dados abaixo não funcionam
            //        o.ApiKey = "388dd3a277cb44c4aa128b5c899a3106"; 
            //        o.LogId = new Guid("c468b2b8-b35d-4f1a-849d-f47b60eef096");
            //    });

            /**
             * adiciona o Elmah como um provider dos logs do aspnet, caso necessário
             * esta ação serve para "juntar" os logs emitidos pela aplicação com o monitoramento da aplicação
             * 
             * pacote instalado: Elmah.Io.Extensions.Logging
             * 
             */
            //services.AddLogging(
            //    builder =>
            //    {
            //        builder.AddElmahIo(o =>
            //        {
            //            // dados de exemplo, os dados abaixo não funcionam
            //            o.ApiKey = "388dd3a277cb44c4aa128b5c899a3106";
            //            o.LogId = new Guid("c468b2b8-b35d-4f1a-849d-f47b60eef096");
            //        });
            //        /**
            //         * recomendações para o filtro
            //         * 
            //         * category = null
            //         * não existe uma categoria definida
            //         * 
            //         * level = LogLevel.Warning
            //         * caso o log level seja definido como information
            //         * poderá ser recebido no monitoramento mais informação que o necessário
            //         */
            //        builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //    });

            #region HealthChecks services
            /**
             * configurações para o heathCheck
             * 
             * o pacote que usa o metodo Add HealthChecks já vem junto com o AspNetCore
             * 
             * Pacote para conexão com banco: AspNetCore.HealthChecks.SqlServer
             * configs:
             * - conexão com o banco de dados
             * - nome que irá definir a conexão, para visualização e organização
             * 
             * metodo -> AddSqlServer()
             * verifica a saúde da conexão com o banco de dados
             * 
             * metodo -> AddCheck()
             * verifica a saude segundo os parametros do check customisado
             * 
             * metodo -> AddElmahIoPublisher()
             * integra logs de HealthChecks com o Elmah com suas options de configuração
             * 
             * AddHealthChecksUI -> trata-se de inicialização pra interface visual do HealthChecks
             * 
             */
            services.AddHealthChecks()
                //.AddElmahIoPublisher(options =>
                //{
                //    options.ApiKey = "388dd3a277cb44c4aa128b5c899a3106";
                //    options.LogId = new Guid("c468b2b8-b35d-4f1a-849d-f47b60eef096");
                //    options.HeartbeatId = "API Fornecedores";
                //})
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            services.AddHealthChecksUI()
                .AddSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            #endregion


            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            //app.UseElmahIo();

            return app;
        }
    }
}
