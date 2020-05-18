using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.IO;

namespace Shared.Configuration
{
    public class RabbitMQServicesConfigurations
    {
        public RabbitMQServicesConfigurations()
        {
            getInstance();
        }

        private void getInstance()
        {
            //if(_configuration==null)
            {                
                RabbitMQConfigurations = getConfigurations();
                Factory = getConnection();
            }            
        }

        private static IConfiguration _configuration;

        public RabbitMQConfigurations RabbitMQConfigurations { get; private set; }
        public ConnectionFactory Factory { get; private set; }

        private RabbitMQConfigurations getConfigurations()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();

            RabbitMQConfigurations = new RabbitMQConfigurations();
            new ConfigureFromConfigurationOptions<RabbitMQConfigurations>(
                _configuration.GetSection("RabbitMQConfigurations"))
                    .Configure(RabbitMQConfigurations);

            return RabbitMQConfigurations;
        }

        private ConnectionFactory getConnection()
        {
            Factory = new ConnectionFactory()
            {
                HostName = RabbitMQConfigurations.HostName,
                Port = RabbitMQConfigurations.Port,
                UserName = RabbitMQConfigurations.UserName,
                Password = RabbitMQConfigurations.Password
            };

            return Factory;
        }
    }

}
