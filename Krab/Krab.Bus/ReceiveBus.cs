using EasyNetQ;
using System;

namespace Krab.Bus
{
    public interface IReceiveBus
    {
        void Subscribe<T>(string subscriptionId, Action<T> receive) where T : class;
    }

    public class ReceiveBus : IReceiveBus
    {
        private readonly string _host;
        private readonly IBus _bus;

        public ReceiveBus()
        {
            _host = "localhost";
            _bus = RabbitHutch.CreateBus($"host={_host}");
        }

        public void Subscribe<T>(string subscriptionId, Action<T> receive) where T : class
        {
            _bus.Subscribe(subscriptionId, receive);
        }
    }
}
