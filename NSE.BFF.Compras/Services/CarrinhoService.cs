﻿using Microsoft.Extensions.Options;
using NSE.BFF.Compras.Extensions;
using NSE.BFF.Compras.Models;
using NSE.Core.Comunication;

namespace NSE.BFF.Compras.Services
{
	public interface ICarrinhoService
	{
		Task<CarrinhoDTO> ObterCarrinho();
		Task<ResponseResult> AdicionarItemCarrinho(ItemCarrinhoDTO produto);
		Task<ResponseResult> AtualizarItemCarrinho(Guid produtoId, ItemCarrinhoDTO carrinho);
		Task<ResponseResult> RemoverItemCarrinho(Guid produtoId);
		Task<ResponseResult> AplicarVoucherCarrinho(VoucherDTO voucher);
	}
	public class CarrinhoService: Service, ICarrinhoService
	{
		private readonly HttpClient _httpClient;

		public CarrinhoService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri(settings.Value.CarrinhoUrl);
		}

		public async Task<CarrinhoDTO> ObterCarrinho()
		{
			var response = await _httpClient.GetAsync("/carrinho/");

			TratarErrosResponse(response);

			return await DeserializarObjetoResponse<CarrinhoDTO>(response);
		}

		public async Task<ResponseResult> AdicionarItemCarrinho(ItemCarrinhoDTO produto)
		{
			var itemContent = ObterConteudo(produto);

			var response = await _httpClient.PostAsync("/carrinho", itemContent);

			if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

			return RetornoOK();
		}

		public async Task<ResponseResult> AtualizarItemCarrinho(Guid produtoId, ItemCarrinhoDTO produto)
		{
			var itemContent = ObterConteudo(produto);

			var response = await _httpClient.PutAsync($"/carrinho/{produto.ProdutoId}", itemContent);

			if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

			return RetornoOK();
		}

		public async Task<ResponseResult> RemoverItemCarrinho(Guid produtoId)
		{
			var response = await _httpClient.DeleteAsync($"/carrinho/{produtoId}");

			if (TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

			return RetornoOK();
		}

        public async Task<ResponseResult> AplicarVoucherCarrinho(VoucherDTO voucher)
        {
			var itemContent = ObterConteudo(voucher);

			var response = await _httpClient.PostAsync("/carrinho/aplicar-voucher/", itemContent);

			if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

			return RetornoOK();
        }
    }
}
