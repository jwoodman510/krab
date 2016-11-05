namespace Krab.Bus
{
    public abstract class Message
    {
        public int RetryAttempt { get; set; }
    }
}
