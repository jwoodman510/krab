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
                    _bus.Send($"{message.GetType()}_Error", message);
                    return;
                }

                Receive(message, subscriber);
            });
        }

        private void Receive<TSubscriber, TMessage>(TMessage message, TSubscriber subscriber)
            where TSubscriber : IMessageSubscriber<TMessage> where TMessage : Message
        {
            try
            {
                _logger.LogInfo($"{message.GetType()} received.");

                subscriber.Receive(message);

                _logger.LogInfo($"{message.GetType()} complete.");
            }
            catch (MessageFailureException ex)
            {
                if (ex.Configuration != null &&
                    ex.Configuration.ShouldRetry &&
                    message.RetryAttempt < ex.Configuration.MaxRetryAttempts)
                {
                    _logger.LogError($"{message.GetType()} failed. Retry attempt {message.RetryAttempt}/{ex.Configuration.MaxRetryAttempts}.", ex);

                    message.RetryAttempt++;
                    Receive(message, subscriber);
                }
                else
                {
                    _logger.LogError($"{message.GetType()} Failed Unexpectedly!", ex);
                    _bus.Send($"{message.GetType()}_Error", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message.GetType()} Failed Unexpectedly!", ex);
                _bus.Send($"{message.GetType()}_Error", message);
            }
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
                    await _bus.SendAsync($"{message.GetType()}_Error", message);
                    return;
                }

                await ReceiveAsync(message, subscriber);
            });
        }

        private async Task ReceiveAsync<TSubscriber, TMessage>(TMessage message, TSubscriber subscriber)
            where TSubscriber : IAsyncMessageSubscriber<TMessage> where TMessage : Message
        {
            try
            {
                _logger.LogInfo($"{message.GetType()} received.");

                await subscriber.ReceiveAsync(message);

                _logger.LogInfo($"{message.GetType()} complete.");
            }
            catch (MessageFailureException ex)
            {
                if (ex.Configuration != null &&
                    ex.Configuration.ShouldRetry &&
                    message.RetryAttempt <= ex.Configuration.MaxRetryAttempts)
                {
                    _logger.LogError($"{message.GetType()} failed. Retry attempt {message.RetryAttempt}/{ex.Configuration.MaxRetryAttempts}.", ex);

                    message.RetryAttempt++;
                    await ReceiveAsync(message, subscriber);
                }
                else
                {
                    _logger.LogError($"{message.GetType()} Failed Unexpectedly!", ex);
                    await _bus.SendAsync($"{message.GetType()}_Error", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{message.GetType()} Failed Unexpectedly!", ex);
                await _bus.SendAsync($"{message.GetType()}_Error", message);
            }
        }

        public void Dispose()
        {
            _bus?.SafeDispose();
        }
    }
}
