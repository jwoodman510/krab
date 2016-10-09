using System.Threading.Tasks;
using System.Web.Mvc;
using Krab.Api;
using Krab.Caching;
using Krab.DataAccess.Dac;
using Krab.DataAccess.User;
using Krab.Global;
using Microsoft.AspNet.Identity;

namespace Krab.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICache _cache;
        private readonly IUserDac _userDac;
        private readonly IAuthApi _authApi;

        public HomeController(IUserDac userDac, ICache cache, IAuthApi authApi)
        {
            _userDac = userDac;
            _cache = cache;
            _authApi = authApi;
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            if (_cache.GetValue<CachedUser>(userId) == null)
            {
                var user = _userDac.Get(userId);
                _cache.SetValue(userId, new CachedUser(user), 60 * 5);
            }
            
            return View();
        }

        public async Task<ActionResult> AuthorizationCallback()
        {
            var userId = User.Identity.GetUserId();

            if (!ValidateAuthorizationCallback(userId))
                return View("Index");

            var code = Request.QueryString["code"];

            if (!string.IsNullOrEmpty(code))
                await _authApi.SaveInitialTokens(code, userId);

            return Redirect("/Home");
        }

        private bool ValidateAuthorizationCallback(string userId)
        {
            if (Request.UrlReferrer?.AbsoluteUri != "https://www.reddit.com/")
                return false;

            var state = Request.QueryString["state"];

            var cachedState = _cache.GetValue<string>(CacheKeys.RedditAuthState(userId));

            if (cachedState != state)
                return false;

            return true;
        }
    }
}
