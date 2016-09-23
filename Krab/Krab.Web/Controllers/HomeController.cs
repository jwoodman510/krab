using System.Web.Mvc;
//using Krab.Cache;
using Krab.DataAccess.Dac;
using Krab.DataAccess.User;
using Microsoft.AspNet.Identity;

namespace Krab.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserDac _userDac;
        //private readonly ICache _cache;

        public HomeController(IUserDac userDac)
        {
            _userDac = userDac;
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var user = _userDac.Get(userId);

            //_cache.SetValue(userId, new CachedUser(user), 60 * 5);

            return View();
        }
    }
}
