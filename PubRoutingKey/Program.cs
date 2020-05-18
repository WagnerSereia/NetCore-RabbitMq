using RabbitMQ.Client;
using Shared.Configuration;
using System;
using System.Text;
using System.Threading;

namespace PubRoutingKey
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
            Console.WriteLine("#................Pub com RountingKey...............#");
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

            #region publisher para o RabbitMQ para a Exchange Direct com routingKey
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: rabbitMQConfigurations.ExchangeNameDirect, type: ExchangeType.Direct);

                while (true)
                {
                    #region Escolha para quais subscriber serão enviados
                    var opcao = -1;
                    while (opcao < 0 || opcao > 2)
                    {
                        Console.WriteLine("Escolha a opcao:");
                        Console.WriteLine("0 - Enviar para todos os susbcriber");
                        Console.WriteLine("1 - Somente para o subscriber 1");
                        Console.WriteLine("2 - Somente para o subscriber 2");
                        opcao = Convert.ToInt32(Console.ReadLine());
                        if (opcao < 0 || opcao > 2)
                            Console.WriteLine("Opcao invalida");
                    }
                    #endregion
                    #region Mensagem a ser enviada
                    Console.WriteLine("Digite a mensagem a ser enviada:");
                    var conteudo = Console.ReadLine();
                    string message =
                        $"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - " +
                        $"Mensagem : {conteudo}, routingKey: {opcao}";
                    var body = Encoding.UTF8.GetBytes(message);
                    #endregion

                    channel.BasicPublish(exchange: rabbitMQConfigurations.ExchangeNameDirect,
                     routingKey: opcao.ToString(),
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
