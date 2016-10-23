using EasyNetQ;
using System;

namespace Krab.Bus
{
    public interface ISendBus : IDisposable
    {
        void Publish<T>(T message) where T : Message;
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

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
