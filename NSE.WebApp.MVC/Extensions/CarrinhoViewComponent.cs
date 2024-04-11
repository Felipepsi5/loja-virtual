using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services;
using NSE.WebApp.MVC.Services.Interfaces;

namespace NSE.WebApp.MVC.Extensions
{
    public class CarrinhoViewComponent : ViewComponent
    {
        private readonly IComprasBFFService _comprasBFFService;

        public CarrinhoViewComponent(IComprasBFFService comprasBFFService)
        {
			_comprasBFFService = comprasBFFService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _comprasBFFService.ObterQuantidadeCarrinho());
        }
    }
}
