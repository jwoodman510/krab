namespace Krab.Bus
{
    public interface IMessageSubscriber<T> where T : class
    {
        void Receive(T message);
    }
}
