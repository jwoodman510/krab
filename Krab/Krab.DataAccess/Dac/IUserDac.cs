using System.Linq;
using Krab.DataAccess.User;

namespace Krab.DataAccess.Dac
{
    public interface IUserDac
    {
        AspNetUser Get(string id);
    }

    public class UserDac : IUserDac
    {
        private readonly UserDb _userDb;
        
        public UserDac(UserDb userDb)
        {
            _userDb = userDb;
        }

        public AspNetUser Get(string id)
        {
            return _userDb.AspNetUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id);
        }
    }
}
