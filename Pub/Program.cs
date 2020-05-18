using RabbitMQ.Client;
using Shared.Configuration;
using System;
using System.Text;
using System.Threading;

namespace Pub
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
            Console.WriteLine("#.......................Pub........................#");
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

            #region publisher para o RabbitMQ para a Exchange Fanout
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: rabbitMQConfigurations.ExchangeNameFanout, 
                                        type: ExchangeType.Fanout);

                while (true)
                {
                    #region Mensagem a ser enviada
                    Console.WriteLine("Digite a mensagem a ser enviada:");
                    var conteudo = Console.ReadLine();
                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Mensagem: {conteudo}";
                    var body = Encoding.UTF8.GetBytes(message);
                    #endregion

                    channel.BasicPublish(exchange: rabbitMQConfigurations.ExchangeNameFanout,
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine("Mensagem enviada com sucesso!");
                }
            }
            #endregion

            _waitHandle.WaitOne();
        }
    }
}
