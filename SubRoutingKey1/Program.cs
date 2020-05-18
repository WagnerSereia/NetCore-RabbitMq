using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configuration;
using System;
using System.Text;
using System.Threading;

namespace SubRoutingKey1
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
            Console.WriteLine("#......................Sub 1.......................#");
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

            #region subscribe do RabbitMQ na Exchange Direct com RoutingKey
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: rabbitMQConfigurations.ExchangeNameDirect, type: ExchangeType.Direct);

                    var queueName = channel.QueueDeclare().QueueName;

                    //Bind com a routingKey das mensagens de opcao 0
                    string opcao = "0";
                    channel.QueueBind(queue: queueName,
                                      exchange: rabbitMQConfigurations.ExchangeNameDirect,
                                      routingKey: opcao);


                    //Bind com a routingKey das mensagens de opcao 1
                    opcao = "1";
                    channel.QueueBind(queue: queueName,
                                      exchange: rabbitMQConfigurations.ExchangeNameDirect,
                                      routingKey: opcao);

                    var consumer1 = new EventingBasicConsumer(channel);
                    consumer1.Received += Sub_ReceivedDirect;
                    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer1);

                    // Aguarda que o evento CancelKeyPress ocorra
                    _waitHandle.WaitOne();
                }
            }
            #endregion
        }
        private static void Sub_ReceivedDirect(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            var routingKey = e.RoutingKey;
            Console.WriteLine(Environment.NewLine + "[mensagem recebida Direct] {0}, da routingKey: {1}", message, routingKey);
        }
    }
}
