using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace ServerStatisticsCollectionService
{
    public class ServerStatisticsCollector : IHostedService, IDisposable
    {
        private readonly IMessageQueue _messageQueue;

        private int _samplingIntervalSeconds;
        private string _serverIdentifier;
        private Timer _timer;
        private double _mb=1024.0*1024.0;

        public void Dispose()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
        }
        public ServerStatisticsCollector(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _samplingIntervalSeconds = AppConfiguration.SamplingIntervalSeconds;
            _serverIdentifier = AppConfiguration.ServerIdentifier;

            _timer = new Timer(CollectAndPublishStatistics, null, TimeSpan.Zero, TimeSpan.FromSeconds(_samplingIntervalSeconds)); ;

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private void CollectAndPublishStatistics(object state)
        {
            var statistics = new ServerStatistics
            {
                MemoryUsage = GetMemoryUsage(),
                AvailableMemory = GetAvailableMemory(),
                CpuUsage = GetCpuUsage(),
                Timestamp = DateTime.UtcNow
            };

            _messageQueue.Publish($"{Topics.ServerStatistics}.{_serverIdentifier}", statistics);
        }
        private double GetMemoryUsage()
        {
            using (var process = Process.GetCurrentProcess())
            {
                return process.PrivateMemorySize64 / _mb; // Convert bytes to MB
            }
        }
        private double GetCpuUsage()
        {
            using (var pc = new PerformanceCounter(Resources.Processor.ToString(), "% Processor Time", "_Total"))
            {
                return pc.NextValue();
            }
        }
        private double GetAvailableMemory()
        {
            using (var pc = new PerformanceCounter(Resources.Memory.ToString(), "Available MBytes"))
            {
                return pc.NextValue();
            }
        }
    }
}