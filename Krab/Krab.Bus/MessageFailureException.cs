using System;

namespace Krab.Bus
{
    public class MessageFailureException : Exception
    {
        public MessageFailureConfiguration Configuration { get; }

        public MessageFailureException(string message, MessageFailureConfiguration configuration = null)
            : base(message)
        {
            Configuration = configuration;
        }

        public MessageFailureException(string message, Exception ex, MessageFailureConfiguration configuration = null)
            : base(message, ex)
        {
            Configuration = configuration;
        }
    }
}
