namespace Krab.Bus
{
    public class MessageFailureConfiguration
    {
        public bool ShouldRetry { get; set; }
        public int MaxRetryAttempts { get; set; }
    }
}
