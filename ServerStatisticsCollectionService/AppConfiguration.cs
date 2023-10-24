using Microsoft.Extensions.Configuration;

namespace ServerStatisticsCollectionService
{
    public static class AppConfiguration
    {
        public static string HostName { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static int SamplingIntervalSeconds { get; set; }
        public static string ServerIdentifier { get; set; }

        public static void Load(IConfiguration configuration)
        {
            HostName = configuration[$"{ConfigurationItem.MessageQueue.ToString()}:{MessageQueueItem.HostName.ToString()}"];
            UserName = configuration[$"{ConfigurationItem.MessageQueue}:{MessageQueueItem.UserName}"];
            Password = configuration[$"{ConfigurationItem.MessageQueue}:{MessageQueueItem.Password}"];
            SamplingIntervalSeconds = Int32.Parse(configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{ConfigurationItem.SamplingIntervalSeconds}"]);
            ServerIdentifier = configuration[$"{ConfigurationItem.ServerStatisticsConfig}:{ConfigurationItem.ServerIdentifier}"];
        }
    }
}