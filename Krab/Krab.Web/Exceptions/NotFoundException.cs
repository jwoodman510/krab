using System.Net;

namespace Krab.Web.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string errorMessage = null)
            : base(HttpStatusCode.NotFound, errorMessage)
        {
        }
    }
}