using Microsoft.Practices.Unity;

namespace Krab.Global
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IAppSettingProvider, AppSettingProvider>();
        }
    }
}
