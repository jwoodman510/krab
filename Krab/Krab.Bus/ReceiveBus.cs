using EasyNetQ;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Krab.Bus
{
    public interface IReceiveBus : IDisposable
    {
        void Subscribe<T>(Action<T> receive) where T : class;

        void RegisterSubscriber<TSubscriber, TMessage>() where TSubscriber : IMessageSubscriber<TMessage> where TMessage : class;
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

        public void Subscribe<T>(Action<T> receive) where T : class
        {
            _bus.Subscribe("Default", receive);
        }

        public void RegisterSubscriber<TSubscriber, TMessage>() where TSubscriber : IMessageSubscriber<TMessage> where TMessage : class
        {
            Subscribe<TMessage>(message => ServiceLocator.Current.GetInstance<TSubscriber>().Receive(message));
        }

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
