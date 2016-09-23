using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.AspNet.Identity;
using Krab.DataAccess.Dac;
using Krab.Global.Extensions;
using Krab.Web.Exceptions;

namespace Krab.Web.Attributes
{
    public class SetUserIdAttribute : ActionFilterAttribute
    {
        private readonly IUserDac _userDac;

        public SetUserIdAttribute()
        {
            _userDac = (IUserDac)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserDac)); ;
        }

        public override void OnActionExecuting(HttpActionContext context)
        {
            var id = HttpContext.Current.User?.Identity?.GetUserId();

            if (id == null)
                return;

            var userId = _userDac.Get(id)?.UserId ?? 0;

            if (userId < 0)
                throw new NotFoundException("User not found");

            HttpContext.Current.SetUserId(userId);
        }
    }
}