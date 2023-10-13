using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;

namespace ServerStatisticsCollectionService
{
    public class RabbitMQMessageQueue : IMessageQueue
    {
        public void Publish(string topic, ServerStatistics statistics)
        {
            var factory = new ConnectionFactory
            {
                HostName = AppConfiguration.HostName,
                UserName = AppConfiguration.UserName,
                Password = AppConfiguration.Password,
            };

            try
            {
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(Topics.ServerStatistics.ToString(), ExchangeType.Topic, durable: true);
                    var body = System.Text.Encoding.UTF8.GetBytes($"{Resources.Memory}={statistics.MemoryUsage}MB, Available {Resources.Memory}={statistics.AvailableMemory}MB, CPU={statistics.CpuUsage}%, Timestamp={statistics.Timestamp}");

                    channel.BasicPublish(exchange: $"{Topics.ServerStatistics}",
                                         routingKey: topic,
                                         basicProperties: null,
                                         body: body);
                    Log.Information($"Published statistics to topic {topic}");
                }
            }catch( Exception ex ) 
            {
                Log.Error($"Error with Connection {ex.Message}");
            }
        }
    }
}