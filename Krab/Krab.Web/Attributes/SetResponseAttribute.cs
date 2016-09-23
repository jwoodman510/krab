using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Krab.Web.Models.Response;

namespace Krab.Web.Attributes
{
    public class SetResponseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var content = context.Response?.Content as ObjectContent;

            if (content?.Value is IOkResponse)
            {
                context.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };
            }
        }
    }
}