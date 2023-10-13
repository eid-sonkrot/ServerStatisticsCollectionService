using Microsoft.Extensions.Configuration;

namespace ServerStatisticsCollectionService
{
    public static class AppConfiguration
    {
        public static string RabbitMQHostName { get; set; }
        public static string RabbitMQUserName { get; set; }
        public static string RabbitMQPassword { get; set; }
        public static int SamplingIntervalSeconds { get; set; }
        public static string ServerIdentifier { get; set; }

        public static void Load(IConfiguration configuration)
        {
            RabbitMQHostName = configuration["RabbitMQ:HostName"];
            RabbitMQUserName = configuration["RabbitMQ:UserName"];
            RabbitMQPassword = configuration["RabbitMQ:Password"];
            SamplingIntervalSeconds = Int32.Parse(configuration["ServerStatisticsConfig:SamplingIntervalSeconds"]);
            ServerIdentifier = configuration["ServerStatisticsConfig:ServerIdentifier"];
        }
    }
}
