using NSE.Core.Data;

namespace NSE.Clientes.API.Models
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        void Adicionar(Cliente cliente);
        Task<IEnumerable<Cliente>> ObterTodos();
        Task<Cliente> ObterPorCpf(string cpf);
        Task<Endereco> ObterEnderecoPorId(Guid id);
        void AdicionarEndereco(Endereco endereco);
    }
}
