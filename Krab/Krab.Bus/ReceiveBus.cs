using EasyNetQ;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Threading.Tasks;
using Krab.Logger;

namespace Krab.Bus
{
    public interface IReceiveBus : IDisposable
    {
        void RegisterSubscriber<TSubscriber, TMessage>() where TSubscriber : IMessageSubscriber<TMessage> where TMessage : Message;

        void RegisterAsyncSubscriber<TSubscriber, TMessage>() where TSubscriber : IAsyncMessageSubscriber<TMessage> where TMessage : Message;
    }

    public class ReceiveBus : IReceiveBus
    {
        private readonly IBus _bus;
        private readonly ILogger _logger;

        public ReceiveBus(string host, ILogger logger)
        {
            _logger = logger ?? new EmptyLogger();
            _bus = RabbitHutch.CreateBus($"host={host}");
        }

        public void Subscribe<T>(Action<T> receive) where T : class
        {
            _bus.Subscribe("Default", receive);
        }

        public void SubscribeAsync<T>(Func<T, Task> receive) where T : class
        {
            _bus.SubscribeAsync("Default", receive);
        }

        public void RegisterSubscriber<TSubscriber, TMessage>() where TSubscriber : IMessageSubscriber<TMessage> where TMessage : Message
        {
            Subscribe<TMessage>(message =>
            {
                TSubscriber subscriber;

                try
                {
                    subscriber = ServiceLocator.Current.GetInstance<TSubscriber>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to get an instance of subscriber: {typeof(TSubscriber).Name}.", ex);
                    throw;
                }

                try
                {
                    _logger.LogInfo($"{message.GetType()} received.");

                    subscriber.Receive(message);

                    _logger.LogInfo($"{message.GetType()} complete.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{message.GetType()} Failed Unexpectedly!", ex);
                    throw;
                }
            });
        }

        public void RegisterAsyncSubscriber<TSubscriber, TMessage>() where TSubscriber : IAsyncMessageSubscriber<TMessage> where TMessage : Message
        {
            SubscribeAsync<TMessage>(async message =>
            {
                TSubscriber subscriber;

                try
                {
                    subscriber = ServiceLocator.Current.GetInstance<TSubscriber>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to get an instance of subscriber: {typeof(TSubscriber).Name}.", ex);
                    throw;
                }

                try
                {
                    _logger.LogInfo($"{message.GetType()} received.");

                    await subscriber.ReceiveAsync(message);

                    _logger.LogInfo($"{message.GetType()} complete.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{message.GetType()} Failed Unexpectedly!", ex);
                    throw;
                }
            });
        }

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
