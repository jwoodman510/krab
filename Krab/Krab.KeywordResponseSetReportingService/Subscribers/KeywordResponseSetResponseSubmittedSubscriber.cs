using System;
using Krab.Bus;
using Krab.DataAccess.Dac;
using Krab.Global;
using Krab.Messages;

namespace Krab.KeywordResponseSetReportingService.Subscribers
{
    public class KeywordResponseSetResponseSubmittedSubscriber : IMessageSubscriber<KeywordResponseSetResponseSubmitted>
    {
        private readonly IKeywordResponseSetSubredditReportDac _dac;
        private readonly int _maxRetryAttempts;

        public KeywordResponseSetResponseSubmittedSubscriber(IKeywordResponseSetSubredditReportDac dac, IAppSettingProvider appSettingProvider)
        {
            _dac = dac;
            _maxRetryAttempts = appSettingProvider.GetInt("MaxRetryAttempts");
        }
        
        public void Receive(KeywordResponseSetResponseSubmitted message)
        {
            try
            {
                _dac.IncrementResponseOrCreate(message.KeywordResponseSetId, message.SubredditId, message.DateTimeUtc);
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
