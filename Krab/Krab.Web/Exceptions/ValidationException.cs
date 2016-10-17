using System.Net;

namespace Krab.Web.Exceptions
{
    public class ValidationException : ApiException
    {
        public ValidationException(string errorMessage = null)
            : base(HttpStatusCode.BadRequest, errorMessage)
        {
        }

        public ValidationException(DataAccess.Exception.ValidationException ex)
            : base(HttpStatusCode.BadRequest, ex.Message)
        {

        }
    }
}