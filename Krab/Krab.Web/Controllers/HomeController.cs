using System.Web.Mvc;
using Krab.Cache;
using Krab.DataAccess.Dac;
using Krab.DataAccess.User;
using Microsoft.AspNet.Identity;

namespace Krab.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICache _cache;
        private readonly IUserDac _userDac;

        public HomeController(ICache cache, IUserDac userDac)
        {
            _cache = cache;
            _userDac = userDac;
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var user = _userDac.Get(userId);

            _cache.SetValue(userId, new CachedUser(user), 60 * 5);

            return View();
        }
    }
}
