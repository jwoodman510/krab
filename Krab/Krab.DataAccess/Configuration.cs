using Krab.DataAccess.Dac;
using Krab.DataAccess.User;
using Microsoft.Practices.Unity;

namespace Krab.DataAccess
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<UserDb, UserDb>();
            container.RegisterType<IUserDac, UserDac>();
        }
    }
}
