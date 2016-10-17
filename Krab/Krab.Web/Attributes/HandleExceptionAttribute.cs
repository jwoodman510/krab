using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using Krab.Web.Exceptions;

namespace Krab.Web.Attributes
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        public override void OnException(HttpActionExecutedContext context)
        {
            #if DEBUG
                if (!(context.Exception is HttpException) && !(context.Exception is DataAccess.Exception.DataAccessException))
                    return;
            #endif

            HttpException httpException;

            if (context.Exception is DataAccess.Exception.NotFoundException)
            {
                httpException = new NotFoundException((DataAccess.Exception.NotFoundException)context.Exception);
            }
            else if(context.Exception is DataAccess.Exception.ValidationException)
            {
                httpException = new ValidationException((DataAccess.Exception.ValidationException)context.Exception);
            }
            else if (context.Exception is DataAccess.Exception.DataAccessException)
            {
                httpException = new HttpException((int)HttpStatusCode.InternalServerError, context.Exception.Message);
            }
            else
            {
                httpException = context.Exception as HttpException
                                ?? new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
            }

            context.Response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)httpException.GetHttpCode(),
                ReasonPhrase = httpException.Message
            };
        }
    }
}