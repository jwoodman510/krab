namespace Krab.Bus
{
    public interface IMessageSubscriber<T> where T : Message
    {
        void Receive(T message);
    }
}
