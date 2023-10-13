using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ServerStatisticsCollectionService
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
    }
}