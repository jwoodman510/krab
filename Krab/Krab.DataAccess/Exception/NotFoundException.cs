namespace Krab.DataAccess.Exception
{
    public class NotFoundException : DataAccessException
    {
        public NotFoundException(string message)
            : base(message)
        {

        }

        public NotFoundException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
