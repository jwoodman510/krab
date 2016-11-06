using System;
using Krab.Bus;
using Krab.DataAccess.Dac;
using Krab.Global;
using Krab.Logger;
using Krab.Messages;

namespace Krab.KeywordResponseSetReportingService.Subscribers
{
    public class KeywordResponseSetResponseSubmittedSubscriber : IMessageSubscriber<KeywordResponseSetResponsesSubmitted>
    {
        private readonly IKeywordResponseSetSubredditReportDac _dac;
        private readonly ILogger _logger;
        private readonly int _maxRetryAttempts;

        public KeywordResponseSetResponseSubmittedSubscriber(IKeywordResponseSetSubredditReportDac dac, IAppSettingProvider appSettingProvider, ILogger logger)
        {
            _dac = dac;
            _logger = logger;
            _maxRetryAttempts = appSettingProvider.GetInt("MaxRetryAttempts");
        }
        
        public void Receive(KeywordResponseSetResponsesSubmitted message)
        {
            try
            {
                _logger.LogInfo($"Report: {message.KeywordResponseSetId}:{message.SubredditId}:{message.DateTimeUtc} has {message.NumResponses} new responses.");
                _dac.IncrementResponseOrCreate(message.KeywordResponseSetId, message.SubredditId, message.DateTimeUtc, message.NumResponses);
            }
            catch (Exception ex)
            {
                throw new MessageFailureException($"{message.GetType()} Failed.", ex, new MessageFailureConfiguration
                {
                    MaxRetryAttempts = _maxRetryAttempts,
                    ShouldRetry = true
                });
            }
        }
    }
}
