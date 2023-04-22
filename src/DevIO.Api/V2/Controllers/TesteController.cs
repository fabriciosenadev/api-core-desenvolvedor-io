using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V2.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : ControllerBase
    {
        private readonly ILogger _logger;
        public TesteController(ILogger<TesteController> logger)
        {
            _logger = logger;
        }
        [MapToApiVersion("2.0")]
        [HttpGet]
        public string Valor()
        {
            // enviando erros para o servidor do Elmah
            try
            {
                var i = 0;
                var result = 42 / i;
            }
            catch (DivideByZeroException e)
            {
                // realiza o envio dos erros para o Elmah
                e.Ship(HttpContext);
            }

            #region logs de desenvolvimento
            _logger.LogTrace("Log de Trace");
            _logger.LogDebug("Log de Debug");
            #endregion

            // grava qualquer coisa que vc queira registar, não importante
            _logger.LogInformation("Log de Informação");

            // não é um erro mas não deveria acontecer
            _logger.LogWarning("Log de Aviso");

            // erro
            _logger.LogError("Log de Erro");

            // ameaça a saúde da aplicação
            _logger.LogCritical("Log de Problema Critico");

            return "Sou a V2";
        }
    }
}
