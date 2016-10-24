using EasyNetQ;
using System;
using System.Threading.Tasks;

namespace Krab.Bus
{
    public interface ISendBus : IDisposable
    {
        void Publish<T>(T message) where T : Message;

        Task PublishAsync<T>(T message) where T : Message;
    }

    public class SendBus : ISendBus
    {
        private readonly IBus _bus;

        public SendBus(string host)
        {
            _bus = RabbitHutch.CreateBus($"host={host}");
        }

        public void Publish<T>(T message) where T : Message
        {
            _bus.Publish(message);
        }

        public async Task PublishAsync<T>(T message) where T : Message
        {
            await _bus.PublishAsync(message);
        }

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
