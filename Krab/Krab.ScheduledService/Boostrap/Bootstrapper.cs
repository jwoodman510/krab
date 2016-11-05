using Krab.Bus;
using Krab.Logger;
using Krab.ScheduledService.Jobs;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using Krab.Global;

namespace Krab.ScheduledService.Boostrap
{
    public static class Bootstrapper
    {
        private static ILogger _logger;

        public static void Configure()
        {
            var container = new UnityContainer();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            RegisterInstances(container);

            _logger.LogInfo($"ClientId: {AppSettings.ClientId}");
        }

        private static void RegisterInstances(IUnityContainer container)
        {
            var busHost = ConfigurationManager.AppSettings["BusHost"];

            container.RegisterInstance(typeof(ISendBus), new SendBus(busHost));

            _logger = new KrabLogger();

            container.RegisterInstance(typeof(ILogger), _logger);

            Configuration.Register(container);
            DataAccess.Configuration.Register(container);
            Caching.Configuration.RegisterRedisCache(container);
            Api.Configuration.Register(container);
            Global.Configuration.Register(container);

            TryGetInstances();
        }

        private static void TryGetInstances()
        {
            var locator = ServiceLocator.Current;
            
            if (locator == null)
            {
                throw new Exception("Unable to find Service Locator!");
            }

            _logger.LogInfo("Verifying instances are bootstrapped...");

            var types = new List<Type>
            {
                typeof(IDeleteLogs),
                typeof(IProcessKeywordResponseSets),
                typeof(ICloseReports)
            };

            foreach (var type in types)
            {
                try
                {
                    locator.GetInstance(type);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Unable to find instane of {type.Name}!");
                    _logger.LogError($"Unable to find instane of {type.Name}!", ex);

#if DEBUG
                    throw;
#endif
                }
            }
        }
    }
}
