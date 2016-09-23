namespace Krab.DataAccess.Exception
{
    public class ValidationException : DataAccessException
    {
        public ValidationException(string message)
            : base(message)
        {

        }

        public ValidationException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
