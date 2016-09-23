namespace Krab.DataAccess.Exception
{
    public class ModificationException : DataAccessException
    {
        public ModificationException(string message)
            : base(message)
        {

        }

        public ModificationException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
