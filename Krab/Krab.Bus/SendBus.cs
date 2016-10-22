using EasyNetQ;
using System;

namespace Krab.Bus
{
    public interface ISendBus : IDisposable
    {
        void Publish(object message);
    }

    public class SendBus : ISendBus
    {
        private readonly string _host;
        private readonly IBus _bus;

        public SendBus()
        {
            _host = "localhost";
            _bus = RabbitHutch.CreateBus($"host={_host}");
        }

        public void Publish(object message)
        {
            _bus.Publish(message);
        }

        public void Dispose()
        {
            _bus?.Dispose();
        }
    }
}
