using Krab.Bus;
using Krab.KeywordResponseSetProcessorService.Subscribers;
using Krab.Messages;
using log4net;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;

namespace Krab.KeywordResponseSetProcessorService.Bootstrap
{
    public static class Bootstrapper
    {
        private static IReceiveBus _receiveBus;

        public static void Configure()
        {
            var container = new UnityContainer();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            RegisterInstances(container);

            StartReceiveBus(container);
        }

        private static void RegisterInstances(IUnityContainer container)
        {
            container.RegisterInstance(typeof(ILog), LogManager.GetLogger("ServiceLogger"));

            container.RegisterType<IMessageSubscriber<ProcessKeywordResponseSet>, ProcessKeywordResponseSetSubscriber>();
            
            TryGetInstances();
        }

        private static void StartReceiveBus(IUnityContainer container)
        {
            _receiveBus = new ReceiveBus();

            _receiveBus.RegisterSubscriber<IMessageSubscriber<ProcessKeywordResponseSet>, ProcessKeywordResponseSet>();

            container.RegisterInstance(typeof(IReceiveBus), _receiveBus);
        }

        public static void StopReceiveBus()
        {
            _receiveBus?.Dispose();
        }

        private static void TryGetInstances()
        {
            var locator = ServiceLocator.Current;
            
            if (locator == null)
            {
                throw new Exception("Unable to find Service Locator!");
            }

            var logger = locator.GetInstance<ILog>();

            logger.Info("Verifying instances are bootstrapped...");

            var types = new List<Type>
            {
                typeof(IMessageSubscriber<ProcessKeywordResponseSet>)
            };

            foreach (var type in types)
            {
                try
                {
                    locator.GetInstance(type);
                }
                catch (Exception ex)
                {
                    logger.Warn($"Unable to find instane of {type.Name}!");
                    logger.Error($"Unable to find instane of {type.Name}!", ex);

#if DEBUG
                    throw;
#endif
                }
            }
        }
    }
}
