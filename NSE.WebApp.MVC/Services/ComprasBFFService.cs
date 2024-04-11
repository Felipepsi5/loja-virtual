using Microsoft.Extensions.Options;
using NSE.Core.Comunication;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using NSE.WebApp.MVC.Services.Interfaces;

namespace NSE.WebApp.MVC.Services
{
    public class ComprasBFFService : Service, IComprasBFFService
	{
        private readonly HttpClient _httpClientComprasBFF;

        public ComprasBFFService(HttpClient httpClient, IOptions<AppSettings> settings)
        {
            _httpClientComprasBFF = httpClient;
            _httpClientComprasBFF.BaseAddress = new Uri(settings.Value.ComprasBFFUrl);
        }

        public async Task<CarrinhoViewModel> ObterCarrinho()
        {
            var response = await _httpClientComprasBFF.GetAsync("/compras/carrinho");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<CarrinhoViewModel>(response);
        }

        public async Task<int> ObterQuantidadeCarrinho()
        {
            var response = await _httpClientComprasBFF.GetAsync("/compras/carrinho-quantidade");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<int>(response);
        }

        public async Task<ResponseResult> AdicionarItemCarrinho(ItemCarrinhoViewModel produto)
        {
            var itemContent = ObterConteudo(produto);

            var response = await _httpClientComprasBFF.PostAsync("/compras/carrinho/items", itemContent);

            if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOK();
        }

        public async Task<ResponseResult> AtualizarItemCarrinho(Guid idProduto, ItemCarrinhoViewModel produto)
        {   

            var itemContent = ObterConteudo(produto);

			var response = await _httpClientComprasBFF.PutAsync($"compras/carrinho/items/{produto.ProdutoId}", itemContent);

            if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOK();
        }

        public async Task<ResponseResult> RemoverItemCarrinho(Guid produtoId)
        {
            var response = await _httpClientComprasBFF.DeleteAsync($"compras/carrinho/items/{produtoId}");

            if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOK();
        }


    }
}
