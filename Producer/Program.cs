using System;
using System.IO;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configuration;

namespace Producer
{
    class Program
    {
        private static readonly AutoResetEvent _waitHandle =
            new AutoResetEvent(false);

        static void Main(string[] args)
        {
            #region Configuracoes do RabbitMq
            var rabbitMqService = new RabbitMQServicesConfigurations();
            var rabbitMQConfigurations = rabbitMqService.RabbitMQConfigurations;
            var factory = rabbitMqService.Factory;
            #endregion

            #region Cabecalho
            Console.WriteLine("####################################################");
            Console.WriteLine("#....................PRODUCER......................#");
            Console.WriteLine("####################################################");
            #endregion

            #region Evento para sair
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Saindo...");

                // Libera a continuação da thread principal
                _waitHandle.Set();
                e.Cancel = true;
            };
            #endregion

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: rabbitMQConfigurations.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                var numero = 20;
                Console.WriteLine($"Serão gerados {numero} mensagens para que os consumidores 1 e 2 recebam as mensagens aleatoriamente");
                Console.WriteLine("Pressione enter para iniciar...");
                Console.ReadLine();

                for (int i = 0; i < numero; i++)
                {
                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Mensagem: {i}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: rabbitMQConfigurations.QueueName,//QueueName pq sera o mesmo nome nos consumers
                                         basicProperties: properties,
                                         body: body);
                }
            }

            _waitHandle.WaitOne();
        }
    }
}
