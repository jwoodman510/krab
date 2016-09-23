using System.Web;
using System.Web.Http;
using Krab.Global.Extensions;
using Krab.Web.Attributes;

namespace Krab.Web.Controllers.Api
{
    [CaptureTiming]
    [Authorize]
    [SetUserId]
    [SetResponse]
    [HandleException]
    public class BaseController : ApiController
    {
        protected int GetUserId()
        {
            return HttpContext.Current.GetUserId();
        }
    }
}