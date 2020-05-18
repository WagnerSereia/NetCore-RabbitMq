using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TopicReceiver2
{
    class Program
    {
        private static readonly AutoResetEvent _waitHandle =
            new AutoResetEvent(false);
        static void Main()
        {
            #region Configuracoes do RabbitMq
            var rabbitMqService = new RabbitMQServicesConfigurations();
            var rabbitMQConfigurations = rabbitMqService.RabbitMQConfigurations;
            var factory = rabbitMqService.Factory;
            #endregion

            #region Cabecalho
            Console.WriteLine("####################################################");
            Console.WriteLine("#...................TOPIC Receiver2................#");
            Console.WriteLine("####################################################");
            Console.WriteLine();
            Console.WriteLine("Este receiver recebera mensagens de:");
            Console.WriteLine("todos.Log.todos");
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
                channel.ExchangeDeclare(exchange: rabbitMQConfigurations.TopicName, type: ExchangeType.Topic);
                var queueName = channel.QueueDeclare().QueueName;

                var routingKeys = new List<string>();
                //routingKeys.Add("w.*.*");
                routingKeys.Add("*.l.*");
                //routingKeys.Add("w.*.c");

                foreach (var bindingKey in routingKeys)
                {
                    channel.QueueBind(queue: queueName, exchange: rabbitMQConfigurations.TopicName, routingKey: bindingKey);
                }

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Topic_Received;
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                // Aguarda que o evento CancelKeyPress ocorra
                _waitHandle.WaitOne();
            }
        }

        private static void Topic_Received(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            var routingKey = e.RoutingKey;
            Console.WriteLine(Environment.NewLine + "[mensagem recebida Topico] {0}, da routingKey: {1}", message, routingKey);
        }
    }
}
