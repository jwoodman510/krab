using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Krab.Web.Models.Response;

namespace Krab.Web.Attributes
{
    public class CaptureTimingAttribute : ActionFilterAttribute
    {
        private Stopwatch _stopwatch;

        public override void OnActionExecuting(HttpActionContext context)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            _stopwatch.Stop();

            var objectContent = context.Response?.Content as ObjectContent;

            var response = objectContent?.Value as IOkResponse;

            if (response == null)
                return;

            response.ServerResponseTimeMs = _stopwatch.ElapsedMilliseconds;
        }
    }
}