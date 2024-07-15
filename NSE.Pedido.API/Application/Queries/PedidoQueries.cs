using Dapper;
using NSE.Pedidos.API.Application.DTO;
using NSE.Pedidos.Domain.Pedidos;
using NSE.Pedidos.Infra.Migrations;

namespace NSE.Pedidos.API.Application.Queries
{
	public interface IPedidoQueries
	{
		Task<PedidoDTO> ObterUltimoPedido(Guid clienteID);
		Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId);
		Task<PedidoDTO> ObterPedidosAutorizados();
	}

	public class PedidoQueries : IPedidoQueries
	{
		private readonly IPedidoRepository _pedidoRepository;

		public PedidoQueries(IPedidoRepository pedidoRepository)
		{
			_pedidoRepository = pedidoRepository;
		}

		public async Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId)
		{
			var pedidos = await _pedidoRepository.ObterListaPorClienteId(clienteId);

			return pedidos.Select(PedidoDTO.ParaPedidoDTO);
		}

		public async Task<PedidoDTO> ObterPedidosAutorizados()
		{
			const string sql = @"SELECT TOP 1
								P.ID as 'PedidoId', P.ID, P.CLIENTEID,
								PI.ID as 'PedidoItemId', PI.ID, PI.PRODUTOID, PI.QUANTIDADE
								FROM PEDIDOS P 
								INNER JOIN PEDIDOITEMS PI ON P.ID = PI.PEDIDOID 
								WHERE P.PEDIDOSTATUS = 1 
								ORDER BY P.DATACADASTRO";

			var lookup = new Dictionary<Guid, PedidoDTO>();

			var pedido = await _pedidoRepository.ObterConexao().QueryAsync<PedidoDTO, PedidoItemDTO, PedidoDTO>(sql, (p, pi) =>
			{
				if (!lookup.TryGetValue(p.Id, out var pedidoDTO))
					lookup.Add(p.Id, pedidoDTO = p);

				pedidoDTO.pedidoItems ??= new List<PedidoItemDTO>();
				pedidoDTO.pedidoItems.Add(pi);

				return pedidoDTO;
			}, splitOn: "PedidoId, PedidoItemId");

			return pedido.FirstOrDefault();
		}

		public async Task<PedidoDTO> ObterUltimoPedido(Guid clienteId)
		{

			var conexao = _pedidoRepository.ObterConexao();

			var pedido = (await conexao.QueryAsync<dynamic>(@"
                                        SELECT TOP(1)
                                            ID AS 'ProdutoId', 
                                            CODIGO, 
                                            VOUCHERUTILIZADO, 
                                            DESCONTO, 
                                            VALORTOTAL,
                                            PEDIDOSTATUS,
                                            LOGRADOURO,
                                            NUMERO, 
                                            BAIRRO, 
                                            CEP, 
                                            COMPLEMENTO, 
                                            CIDADE, 
                                            ESTADO
                                        FROM PEDIDOS
                                        WHERE 
                                            CLIENTEID = @clienteId AND
                                            PEDIDOSTATUS = 1 
                                        ORDER BY DATACADASTRO DESC
        ", new { clienteId })).FirstOrDefault();

			if (pedido == null)
				return null;

			var items = await conexao.QueryAsync<dynamic>(@"
            SELECT 
                PRODUTONOME,
                VALORUNITARIO,
                QUANTIDADE,
                PRODUTOIMAGEM
            FROM PEDIDOITEMS WHERE PEDIDOID = @pedidoId
        ", new { pedidoId = pedido.ProdutoId });

			return MapearPedido(pedido, items);


		}

		private PedidoDTO MapearPedido(dynamic pedido, IEnumerable<dynamic> items)
		{
			return new PedidoDTO
			{
				Codigo = pedido.CODIGO,
				Status = pedido.PEDIDOSTATUS,
				ValorTotal = pedido.VALORTOTAL,
				Desconto = pedido.DESCONTO,
				VoucherUtilizado = pedido.VOUCHERUTILIZADO,
				Endereco = new EnderecoDTO
				{
					Logradouro = pedido.LOGRADOURO,
					Bairro = pedido.BAIRRO,
					Cep = pedido.CEP,
					Cidade = pedido.CIDADE,
					Complemento = pedido.COMPLEMENTO,
					Estado = pedido.ESTADO,
					Numero = pedido.NUMERO
				},
				pedidoItems = items.Select(item => new PedidoItemDTO
				{
					Nome = item.PRODUTONOME,
					Valor = item.VALORUNITARIO,
					Quantidade = item.QUANTIDADE,
					Imagem = item.PRODUTOIMAGEM
				}).ToList()
			};
		}
	}
}
