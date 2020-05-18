using System;
using System.Text;
using System.Threading;

using Shared.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer1
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
            Console.WriteLine("#....................CONSUMER 1....................#");
            Console.WriteLine("####################################################");
            Console.WriteLine("Aguardando mensagens para processamento");
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

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>
                {
                    var tempoExecucao = new Random().Next(5);

                    var message = Encoding.UTF8.GetString(ea.Body);
                    Console.WriteLine(Environment.NewLine +
                        $"[Nova mensagem recebida] {message} executada em {tempoExecucao} segundos");
                    Thread.Sleep(tempoExecucao * 1000);

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: rabbitMQConfigurations.QueueName,
                     autoAck: false,
                     consumer: consumer);


                // Aguarda que o evento CancelKeyPress ocorra
                _waitHandle.WaitOne();
            }
        }
        
        private static void Consumer_Received(
            object sender, BasicDeliverEventArgs e)
        {
            var tempoExecucao = new Random().Next(5);

            var message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(Environment.NewLine +
                $"[Nova mensagem recebida] {message} executada em {tempoExecucao} segundos");
            Thread.Sleep(tempoExecucao*1000);
        }
    }
}
