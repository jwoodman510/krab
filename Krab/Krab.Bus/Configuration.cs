using System;
using System.Linq;
using Krab.Logger;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Krab.Bus
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<ISendBus, SendBus>();
            container.RegisterType<IReceiveBus, ReceiveBus>();
        }

        public static void TryGetMessageSubscribersInContainingAssembly<T>() where T : class
        {
            var locator = ServiceLocator.Current;

            if (locator == null)
            {
                throw new Exception("Unable to find Service Locator!");
            }

            var logger = locator.GetInstance<ILogger>();

            logger.LogInfo("Verifying instances are bootstrapped...");

            var types = typeof(T)
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
