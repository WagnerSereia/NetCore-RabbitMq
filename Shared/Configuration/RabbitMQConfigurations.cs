using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Configuration
{
    public class RabbitMQConfigurations
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public string ExchangeNameFanout { get; set; }
        public string ExchangeNameDirect { get; set; }
        public string TopicName { get; set; }
    }
}
