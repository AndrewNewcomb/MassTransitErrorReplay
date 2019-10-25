namespace Subscriber
{
    public class SubscriberParams : Common.BaseParams
    {
        public string Name { get; set; } = "invSubscriber";
        public bool DisableQueueOutbox { get; set; } = false;
        public bool QueueIsExclusive { get; set; } = false;
        public bool DisableQueueRetry { get; set; } = false;
        public bool DisableQueue2LevelRetry { get; set; } = false;
        public bool DisableFaultQueue { get; set; } = false;
        public bool DisableNoteworthyQueue { get; set; } = false;
    }
}
