using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Krab.Web.Attributes
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        public override void OnException(HttpActionExecutedContext context)
        {
#if DEBUG
            if (!(context.Exception is HttpException))
                return;
#endif

            var httpException = context.Exception as HttpException
                                ?? new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");

            context.Response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)httpException.GetHttpCode(),
                ReasonPhrase = httpException.Message
            };
        }
    }
}