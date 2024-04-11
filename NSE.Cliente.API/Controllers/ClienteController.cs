using Microsoft.AspNetCore.Mvc;
using NSE.Clientes.API.Application.Commands;
using NSE.Core.Mediator;
using NSE.Web.Api.Core.Controllers;

namespace NSE.Clientes.API.Controllers
{
    public class ClienteController : MainController
    {

        private readonly IMediatorHandler _mediatorHandler;

        public ClienteController(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler; 
        }

        [HttpGet("clientes")]
        public async Task<IActionResult> Index()
        {
            var resultado = await _mediatorHandler.EnviarComando(
                new RegistrarClienteCommand(Guid.NewGuid(), "Felipe", "felipe@pedu.com", "73257934050"));

            return CustomResponse(resultado);
        }
    }
}
