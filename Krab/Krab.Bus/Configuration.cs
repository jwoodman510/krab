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
    }
}
