using Krab.Bus;
using Krab.KeywordResponseSetProcessorService.Subscribers;
using Krab.Logger;
using Krab.Messages;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Configuration;
using System.Linq;
using Krab.Global;

namespace Krab.KeywordResponseSetProcessorService.Bootstrap
{
    public static class Bootstrapper
    {
        private static IReceiveBus _receiveBus;
        private static ILogger _logger;

        public static void Configure()
        {
            var container = new UnityContainer();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            RegisterInstances(container);

            StartReceiveBus(container);

            _logger.LogInfo($"ClientId: {AppSettings.ClientId}");
        }

        private static void RegisterInstances(IUnityContainer container)
        {
            _logger = new KrabLogger();
            container.RegisterInstance(typeof(ILogger), _logger);

            _logger.LogInfo("Registering Instances...");

            DataAccess.Configuration.Register(container);
            Api.Configuration.Register(container);
            Global.Configuration.Register(container);
            Caching.Configuration.RegisterRedisCache(container);

            container.RegisterType<IMessageSubscriber<ProcessKeywordResponseSet>, ProcessKeywordResponseSetSubscriber>();

            TryGetInstances();
        }

        private static void StartReceiveBus(IUnityContainer container)
        {
            _logger.LogInfo("Starting Receive Bus...");

            var busHost = ConfigurationManager.AppSettings["BusHost"];

            _receiveBus = new ReceiveBus(busHost, _logger);

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

            var logger = locator.GetInstance<ILogger>();

            logger.LogInfo("Verifying instances are bootstrapped...");

            var types = typeof(Bootstrapper)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t =>
                {
                    var interfaces = t.GetInterfaces();

                    foreach (var i in interfaces)
                    {
                        if (i.IsGenericType)
                        {
                            var typeDef = i.GetGenericTypeDefinition();
                            var iDef = typeof(IMessageSubscriber<>).GetGenericTypeDefinition();

                            if (typeDef == iDef)
                                return true;
                        }
                    }

                    return false;
                });

            foreach (var type in types)
            {
                try
                {
                    locator.GetInstance(type);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Unable to find instane of {type.Name}!");
                    logger.LogError($"Unable to find instane of {type.Name}!", ex);

#if DEBUG
                    throw;
#endif
                }
            }
        }
    }
}
