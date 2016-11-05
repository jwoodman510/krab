using Krab.Bus;
using Krab.Logger;
using Krab.Messages;

namespace Krab.KeywordResponseSetReportingService.Subscribers
{
    public class KeywordResponseSetResponseSubmittedSubscriber : IMessageSubscriber<KeywordResponseSetResponseSubmitted>
    {
        private readonly ILogger _logger;

        public KeywordResponseSetResponseSubmittedSubscriber(ILogger logger)
        {
            _logger = logger;
        }

        public void Receive(KeywordResponseSetResponseSubmitted message)
        {

        }
    }
}
