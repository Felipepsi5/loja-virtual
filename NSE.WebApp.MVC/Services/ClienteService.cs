using Microsoft.Extensions.Options;
using NSE.Core.Comunication;
using NSE.WebAPI.Core.Usuario;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System.Net;

namespace NSE.WebApp.MVC.Services
{
    public interface IClienteService
    {
        Task<EnderecoViewModel> ObterEndereco();

        Task<ResponseResult> AdicionarEndereco(EnderecoViewModel endereco);
    }

    public class ClienteService : Service, IClienteService
    {
        private readonly HttpClient _httpClient;
        private readonly IAspNetUser _user;

        public ClienteService(IAspNetUser user, IOptions<AppSettings> settings, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _user = user;
            _httpClient.BaseAddress = new Uri(settings.Value.ClienteUrl);
        }

        public async Task<EnderecoViewModel> ObterEndereco()
        {
            var response = await _httpClient.GetAsync("/cliente/endereco/");

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            TratarErrosResponse(response);

            return await DeserializarObjetoResponse<EnderecoViewModel>(response);
        }

        public async Task<ResponseResult> AdicionarEndereco(EnderecoViewModel endereco)
        {
            try
			{
				var enderecoContent = ObterConteudo(endereco);

				var response = await _httpClient.PostAsync("/cliente/endereco/", enderecoContent);

				if (!TratarErrosResponse(response)) return await DeserializarObjetoResponse<ResponseResult>(response);

				return RetornoOK();
			}
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
