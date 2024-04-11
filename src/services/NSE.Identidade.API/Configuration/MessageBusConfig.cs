using NSE.Core.Utils;
using NSE.MessageBus;

namespace NSE.Identidade.API.Configuration
{
    public static class MessageBusConfig
    {

        public static void AddMessageBusConfiguration(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var resultado = configuration.GetMessageQueueConnection("MessageBus");


            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
        }
    }
}
