using Krab.Bus;
using Krab.KeywordResponseSetProcessorService.Subscribers;
using Krab.Logger;
using Krab.Messages;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Configuration;
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

            StartBus(container);

            Bus.Configuration.TryGetMessageSubscribersInContainingAssembly<ProcessKeywordResponseSetSubscriber>();

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
        }

        private static void StartBus(IUnityContainer container)
        {
            _logger.LogInfo("Starting Receive Bus...");

            var busHost = ConfigurationManager.AppSettings["BusHost"];

            _receiveBus = new ReceiveBus(busHost, _logger);
            
            _receiveBus.RegisterSubscriber<IMessageSubscriber<ProcessKeywordResponseSet>, ProcessKeywordResponseSet>();

            container.RegisterInstance(typeof(IReceiveBus), _receiveBus);
            container.RegisterInstance(typeof(ISendBus), new SendBus(busHost));
        }

        public static void StopReceiveBus()
        {
            _receiveBus?.Dispose();
        }
    }
}
