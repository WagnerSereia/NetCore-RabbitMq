using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configuration;
using System;
using System.Text;
using System.Threading;

namespace Sub1
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

            #region subscribe do RabbitMQ na Exchange Fanout            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: rabbitMQConfigurations.ExchangeNameFanout,
                                        type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: rabbitMQConfigurations.ExchangeNameFanout,
                                  routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Sub_ReceivedFanout;
                channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

                // Aguarda que o evento CancelKeyPress ocorra
                _waitHandle.WaitOne();
            }
            #endregion
        }

        private static void Sub_ReceivedFanout(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(Environment.NewLine + "[mensagem recebida Fanout] {0}", message);
        }
    }
}
