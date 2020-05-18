using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPCServer
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
            Console.WriteLine("#...................Server.........................#");
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

            #region subscribe do RabbitMQ na Queue de RPC            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: rabbitMQConfigurations.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: rabbitMQConfigurations.QueueName, autoAck: false, consumer: consumer);

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        int n = int.Parse(message);
                        Console.WriteLine(" [.] fib({0})", message);
                        response = fib(n).ToString();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };

                // Aguarda que o evento CancelKeyPress ocorra
                _waitHandle.WaitOne();
            }
            #endregion
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1)
                return n;
            
            return fib(n - 1) + fib(n - 2);
        }
    }
}
