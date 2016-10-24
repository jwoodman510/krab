using System.Threading.Tasks;

namespace Krab.Bus
{
    public interface IMessageSubscriber<T> where T : Message
    {
        void Receive(T message);
    }

    public interface IAsyncMessageSubscriber<T> where T : Message
    {
        Task ReceiveAsync(T message);
    }
}
