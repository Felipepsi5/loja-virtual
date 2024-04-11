﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.BFF.Compras.Models;
using NSE.BFF.Compras.Services;
using NSE.Web.Api.Core.Controllers;

namespace NSE.BFF.Compras.Controllers
{
	[Authorize]
	public class CarrinhoController : MainController
	{
		private readonly ICarrinhoService _carrinhoService;
		private readonly ICatalogoService _catalogoService;

		public CarrinhoController(ICarrinhoService carrinhoService, ICatalogoService catalogoService)
		{
			_carrinhoService = carrinhoService;
			_catalogoService = catalogoService;
		}

		[HttpGet]
		[Route("compras/carrinho")]
		public async Task<IActionResult> Index()
		{
			return CustomResponse(await _carrinhoService.ObterCarrinho());
		}

		[HttpGet]
		[Route("compras/carrinho-quantidade")]
		public async Task<int> ObterQuantidadeCarrinho()
		{
			var quantidade = await _carrinhoService.ObterCarrinho();
			return quantidade?.Itens.Sum(i => i.Quantidade) ?? 0;
		}

		[HttpPost]
		[Route("compras/carrinho/items")]
		public async Task<IActionResult> AdicionarItemCarrinho(ItemCarrinhoDTO itemProduto)
		{
			var produto = await _catalogoService.ObterPorId(itemProduto.ProdutoId);
			await ValidaItemCarrinho(produto, itemProduto.Quantidade);
			if (!OperacaoValida()) return CustomResponse();

			itemProduto.Nome = produto.Nome;
			itemProduto.Valor = produto.Valor;
			itemProduto.Imagem = produto.Imagem;

			var resposta = await _carrinhoService.AdicionarItemCarrinho(itemProduto);

			return CustomResponse(resposta);
		}

		[HttpPut]
		[Route("compras/carrinho/items/{produtoId}")]
		public async Task<IActionResult> AtualizarItemCarrinho(Guid produtoId, ItemCarrinhoDTO itemProduto)
		{
			var produto = await _catalogoService.ObterPorId(produtoId);

			await ValidaItemCarrinho(produto, itemProduto.Quantidade);
			if (!OperacaoValida()) return CustomResponse();

			itemProduto.Imagem = produto.Imagem;
			itemProduto.Valor = produto.Valor;
			itemProduto.Nome = produto.Nome;

			var resposta = await _carrinhoService.AtualizarItemCarrinho(produtoId, itemProduto);

			if(!ResponsePossuiErros(resposta))
				return CustomResponse(resposta);

			return CustomResponse();
		}

		[HttpDelete]
		[Route("compras/carrinho/items/{produtoId}")]
		public async Task<IActionResult> RemoverItemCarrinho(Guid produtoId)
		{
			var produto = await _catalogoService.ObterPorId(produtoId);

			if(produto == null)
			{
				AdicionarErroProcessamento("Produto inexistente!");
				return CustomResponse();
			}

			var resposta = await _carrinhoService.RemoverItemCarrinho(produtoId);

			return CustomResponse(resposta);
		}


		private async Task ValidaItemCarrinho(ItemProdutoDTO produto, int quantidade)
		{
			if (produto == null) AdicionarErroProcessamento("Produto inexistente!");
			if (quantidade < 1) AdicionarErroProcessamento($"Escolha ao menos uma unidade do produto {produto.Nome}");

			var carrinho = await _carrinhoService.ObterCarrinho();
			var itemCarrinho = carrinho.Itens.FirstOrDefault(p => p.ProdutoId == produto.Id);

			if(itemCarrinho != null && itemCarrinho.Quantidade + quantidade > produto.QuantidadeEstoque)
			{
				AdicionarErroProcessamento($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades em estoque," +
										   $" você selecionou {quantidade}");
				return;
			}

			if (quantidade > produto.QuantidadeEstoque) AdicionarErroProcessamento($"O produto {produto.Nome} possui {produto.QuantidadeEstoque} unidades");
		}
	}	
}
