using System.Net;

namespace Krab.Web.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string errorMessage = null)
            : base(HttpStatusCode.NotFound, errorMessage)
        {
        }

        public NotFoundException(DataAccess.Exception.NotFoundException ex)
            : base(HttpStatusCode.NotFound, ex.Message)
        {
            
        }
    }
}