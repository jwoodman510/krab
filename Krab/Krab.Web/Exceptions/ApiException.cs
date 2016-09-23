using System.Net;
using System.Web;

namespace Krab.Web.Exceptions
{
    public class ApiException : HttpException
    {
        public ApiException(HttpStatusCode statusCode, string errorMessage = null)
            : base((int)statusCode, errorMessage)
        {

        }
    }
}