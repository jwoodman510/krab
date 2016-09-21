using System.Web.Mvc;
using Krab.Cache;
using Microsoft.AspNet.Identity;

namespace Krab.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICache _cache;
        public HomeController(ICache cache)
        {
            _cache = cache;
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();



            return View();
        }
    }
}
