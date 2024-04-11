using NSE.BFF.Compras.Extensions;
using NSE.BFF.Compras.Services;
using NSE.Web.Api.Core.Extensions;
using NSE.WebAPI.Core.Usuario;
using Polly;

namespace NSE.BFF.Compras.Configuration
{
    public static class DepedencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

			services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

			services.AddHttpClient<ICatalogoService, CatalogoService>()
					.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
					.AddPolicyHandler(PollyExtensions.EsperarTentar())
					.AddTransientHttpErrorPolicy(
					   p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

			services.AddHttpClient<ICarrinhoService, CarrinhoService>()
					 .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
					 .AddPolicyHandler(PollyExtensions.EsperarTentar())
					 .AddTransientHttpErrorPolicy(
						p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
		}
    }
}
