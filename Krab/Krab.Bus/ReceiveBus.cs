using EasyNetQ;
using System;

namespace Krab.Bus
{
    public interface IReceiveBus : IDisposable
    {
        void Subscribe<T>(Action<T> receive) where T : class;
    }

    public class ReceiveBus : IReceiveBus
    {
        private readonly string _host;
        private readonly IBus _bus;

        public ReceiveBus()
        {
            _host = "localhost";
            _bus = RabbitHutch.CreateBus($"host={_host};publisherConfirms=true;timeout=10");
        }

        public void Subscribe<T>(Action<T> receive) where T : class
        {
            _bus.Subscribe("Default", receive);
        }

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
