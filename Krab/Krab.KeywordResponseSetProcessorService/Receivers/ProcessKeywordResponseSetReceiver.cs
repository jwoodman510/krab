using Krab.Messages;
using log4net;

namespace Krab.KeywordResponseSetProcessorService.Receivers
{
    public class ProcessKeywordResponseSetReceiver
    {
        private readonly ILog _logger;

        public ProcessKeywordResponseSetReceiver(ILog logger)
        {
            _logger = logger;
        }

        public void Receive(ProcessKeywordResponseSet message)
        {
            _logger.Info($"{message.GetType()} Received. KeywordResponseSetId={message.Id}.");
        }
    }
}
