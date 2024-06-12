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

        public async Task<ResponseResult> AplicarVoucherCarrinho(string voucher)
        {
            var itemContent = ObterConteudo(voucher);

            var response = await _httpClientComprasBFF.PostAsync("/compras/carrinho/aplicar-voucher/", itemContent);

            if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOK();
        }

        public async Task<ResponseResult> FinalizarPedido(PedidoTransacaoViewModel pedidoTransacao)
        {

            var pedidoContent = ObterConteudo(pedidoTransacao);

            var response = await _httpClientComprasBFF.PostAsync($"compras/pedido/", pedidoContent);

            if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

            return RetornoOK();
        }

        public async Task<PedidoViewModel> ObterUltimoPedido()
        {

            var response = await _httpClientComprasBFF.GetAsync("/compras/pedido/ultimo");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<PedidoViewModel>(response);
        }

        public async Task<IEnumerable<PedidoViewModel>> ObterListaPorClienteId()
        {

            var response = await _httpClientComprasBFF.GetAsync("/compras/pedido/lista-cliente");

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<IEnumerable<PedidoViewModel>>(response);
        }

        public PedidoTransacaoViewModel MapearParaPedido(CarrinhoViewModel carrinho, EnderecoViewModel endereco)
        {
            var pedido = new PedidoTransacaoViewModel
            {
                ValorTotal = carrinho.ValorTotal,
                Itens = carrinho.Itens,
                Desconto = carrinho.Desconto,
                VoucherUtilizado = carrinho.VoucherUtilizado,
                VoucherCodigo = carrinho.Voucher?.Codigo
            };

            if(endereco != null)
            {
                pedido.Endereco = new EnderecoViewModel
                {
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero,
                    Bairro = endereco.Bairro,
                    Cep = endereco.Cep,
                    Complemento = endereco.Complemento,
                    Cidade = endereco.Cidade,   
                    Estado = endereco.Estado,   
                };
            }

            return pedido;
        }
    }
}
