using EasyNetQ;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;

namespace Krab.Bus
{
    public interface IReceiveBus : IDisposable
    {
        void RegisterSubscriber<TSubscriber, TMessage>() where TSubscriber : IMessageSubscriber<TMessage> where TMessage : Message;
    }

    public class ReceiveBus : IReceiveBus
    {
        private readonly IBus _bus;

        public ReceiveBus(string host)
        {
            _bus = RabbitHutch.CreateBus($"host={host}");
        }

        public void Subscribe<T>(Action<T> receive) where T : class
        {
            _bus.Subscribe("Default", receive);
        }

        public void RegisterSubscriber<TSubscriber, TMessage>() where TSubscriber : IMessageSubscriber<TMessage> where TMessage : Message
        {
            Subscribe<TMessage>(message => ServiceLocator.Current.GetInstance<TSubscriber>().Receive(message));
        }

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
