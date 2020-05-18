using RabbitMQ.Client;
using Shared.Configuration;
using System;
using System.Text;
using System.Threading;

namespace TopicProducer
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
            Console.WriteLine("#...................TOPIC Producer.................#");
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
                channel.ExchangeDeclare(exchange: rabbitMQConfigurations.TopicName,
                                        type: ExchangeType.Topic);
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Em topicos e necessario o uso de routingKey, e a estrutura da rota utilizada no exemplo sera:");
                    Console.WriteLine("servidor.tipoMensagem.severidade");

                    Console.WriteLine("1) O consumerTopic1 - recebera todas as mensagens do servidor: WEB");
                    Console.WriteLine("2) O consumerTopic2 - recebera todas as mensagens do tipoMensagem: Log");
                    Console.WriteLine("3) O consumerTopic3 - recebera todas as mensagens do servidor: WEB e severidade: Critical");

                    Console.WriteLine("Digite a opcao para servidor");
                    Console.WriteLine("w) Web");
                    Console.WriteLine("d) BancoDados");
                    Console.WriteLine("a) Arquivos");
                    var servidor = Console.ReadLine();

                    Console.WriteLine("Digite a opcao para tipoMensagem");
                    Console.WriteLine("l) Log");
                    Console.WriteLine("e) Email");
                    var tipoMensagem = Console.ReadLine();

                    Console.WriteLine("Digite a opcao para severidade");
                    Console.WriteLine("c) Critical");
                    Console.WriteLine("w) Warning");
                    var severidade = Console.ReadLine();

                    var routingKey = $"{servidor}.{tipoMensagem}.{severidade}";

                    Console.WriteLine($"RoutingKey enviado: {routingKey}");

                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Mensagem gerada para {routingKey}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: rabbitMQConfigurations.TopicName,
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);
                }
            }

            _waitHandle.WaitOne();
        }
    }
}
