namespace Krab.DataAccess.Exception
{
    public abstract class DataAccessException : System.Exception
    {
        protected DataAccessException(string message)
            : base(message)
        {

        }

        protected DataAccessException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
