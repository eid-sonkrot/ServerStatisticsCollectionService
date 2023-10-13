namespace ServerStatisticsCollectionService
{
    public interface IMessageQueue
    {
        void Publish(string topic, ServerStatistics statistics);
    }
}
