using Krab.Bus;
using Krab.Messages;
using log4net;

namespace Krab.KeywordResponseSetProcessorService.Subscribers
{
    public class ProcessKeywordResponseSetSubscriber : IMessageSubscriber<ProcessKeywordResponseSet>
    {
        private readonly ILog _logger;

        public ProcessKeywordResponseSetSubscriber(ILog logger)
        {
            _logger = logger;
        }

        public void Receive(ProcessKeywordResponseSet message)
        {
            _logger.Info($"{message.GetType()} Received. KeywordResponseSetId={message.Id}.");
        }
    }
}
