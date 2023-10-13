using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ServerStatisticsCollectionService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ServerStatistics = new Program();
            var currentDirectory=ServerStatistics.GetProjectDirectory();
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IMessageQueue, RabbitMQMessageQueue>();
                services.AddHostedService<ServerStatisticsCollector>();
            });
            var configuration = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            var host = builder.Build();
            // Load the configuration into the static class
            AppConfiguration.Load(configuration);
            ServerStatistics.LoggerConfiguration();
            await host.RunAsync();
        }
        public void LoggerConfiguration()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            Log.Logger = new LoggerConfiguration()
                        .WriteTo.File(Path.Combine(currentDirectory, "Information.log"),
                         restrictedToMinimumLevel: LogEventLevel.Information,
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)
                        .WriteTo.File(Path.Combine(currentDirectory, "Error.log"),
                         restrictedToMinimumLevel: LogEventLevel.Error,
                         rollingInterval: RollingInterval.Day,
                         rollOnFileSizeLimit: true)
                         .CreateLogger();
        }
        public string GetProjectDirectory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            while (!File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
            {
                DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory);
                if (parentDirectory == null)
                {
                    throw new Exception("appsettings.json not found in the directory tree.");
                }
                currentDirectory = parentDirectory.FullName;
            }
            return currentDirectory;
        }
    }
}