﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.Global;
using NCron;
using Krab.Bus;
using Krab.Caching;
using Krab.Messages;
using Krab.Logger;

namespace Krab.ScheduledService.Jobs
{
    public interface IProcessKeywordResponseSets : ICronJob
    {
        
    }

    public class ProcessKeywordResponseSets : CronJob, IProcessKeywordResponseSets
    {
        private readonly ILogger _logger;
        private readonly IRedditUserDac _redditUserDac;
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;
        private readonly IAppSettingProvider _appSettingProvider;
        private readonly ISendBus _sendBus;
        private readonly ICache _cache;

        public ProcessKeywordResponseSets(
            ILogger logger,
            IRedditUserDac redditUserDac,
            IKeywordResponseSetDac keywordResponseSetDac,
            IAppSettingProvider appSettingProvider,
            ISendBus sendBus,
            ICache cache)
        {
            _logger = logger;
            _redditUserDac = redditUserDac;
            _keywordResponseSetDac = keywordResponseSetDac;
            _appSettingProvider = appSettingProvider;
            _sendBus = sendBus;
            _cache = cache;
        }

        public override void Execute()
        {
            _logger.LogInfo($"Executing {GetType()}.");

            try
            {
                var maxDegreeOfParallelism = _appSettingProvider.GetInt("MaxDegreeOfParallelism");

                ProcessSets(maxDegreeOfParallelism);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{GetType()} Failed Unexpectedly.", ex);
            }
            finally
            {
                _logger.LogInfo($"{GetType()} Complete.");
            }
        }
        
        private void ProcessSets(int maxDegreeOfParallelism)
        {
            var userKrSets = _keywordResponseSetDac
                .GetAll()
                .GroupBy(s => s.UserId)
                .Where(ShouldProcessUserSets);

            Parallel.ForEach(userKrSets, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, ProcessUserSets);
        }

        private bool ShouldProcessUserSets(IGrouping<int, KeywordResponseSet> grouping)
        {
            var userId = grouping.Key;

            var redditUser = _redditUserDac.GetByUser(userId)?.FirstOrDefault();

            if (redditUser == null)
                _logger.LogInfo($"UserId: {userId} has no linked reddit account and will not be processed.");

            return redditUser != null;
        }

        private void ProcessUserSets(IGrouping<int, KeywordResponseSet> grouping)
        {
            var cacheKey = $"RedditUser:{grouping.Key}";

            var redditUser = _cache.GetValue<DataAccess.RedditUser.RedditUser>(cacheKey);

            if (redditUser == null)
            {
                redditUser = _redditUserDac.GetByUser(grouping.Key).FirstOrDefault();
                _cache.SetValue(cacheKey, redditUser, 60 * 5);
            }

            if (redditUser == null)
            {
                _logger.LogInfo($"User has no linked Reddit account. Skipping.");
                return;
            }

            _logger.LogInfo($"Processing sets for UserId: {grouping.Key}.");

            foreach (var set in grouping.Where(s => s.Status == KeywordResponseSetStatus.Active.ToString()))
            {
                _logger.LogInfo($"Publishing ProcessKeywordResponseSet message. KeywordResponseSetId={set.Id}.");

                _sendBus.Publish(new ProcessKeywordResponseSet
                {
                    UserId = grouping.Key,
                    RedditUserId = redditUser.Id,
                    RedditUserName = redditUser.UserName,
                    Id = set.Id,
                    StatusId = set.StatusId,
                    Keyword = set.Keyword,
                    Response = set.Response
                });

            }
        }
    }
}

