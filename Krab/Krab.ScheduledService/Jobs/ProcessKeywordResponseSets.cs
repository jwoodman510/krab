using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.DataAccess.Subreddit;
using Krab.Global;
using log4net;
using Microsoft.Practices.ServiceLocation;
using NCron;

namespace Krab.ScheduledService.Jobs
{
    public interface IProcessKeywordResponseSets : ICronJob
    {
        
    }

    public class ProcessKeywordResponseSets : CronJob, IProcessKeywordResponseSets
    {
        private readonly ILog _logger;
        private readonly IRedditUserDac _redditUserDac;
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;
        private readonly IAppSettingProvider _appSettingProvider;

        public ProcessKeywordResponseSets(
            ILog logger,
            IRedditUserDac redditUserDac,
            IKeywordResponseSetDac keywordResponseSetDac,
            IAppSettingProvider appSettingProvider)
        {
            _logger = logger;
            _redditUserDac = redditUserDac;
            _keywordResponseSetDac = keywordResponseSetDac;
            _appSettingProvider = appSettingProvider;
        }

        public override void Execute()
        {
            _logger.Info($"Executing {GetType()}.");

            try
            {
                var shouldParallelize = _appSettingProvider.GetBool("ShouldParallelizeProcessing");
                var maxDegreeOfParallelism = _appSettingProvider.GetInt("MaxDegreeOfParallelism");

                ProcessSets(shouldParallelize, maxDegreeOfParallelism);
            }
            catch (Exception ex)
            {
                _logger.Error($"{GetType()} Failed Unexpectedly.", ex);
            }
            finally
            {
                _logger.Info($"{GetType()} Complete.");
            }
        }
        
        private void ProcessSets(bool shouldParallelize, int maxDegreeOfParallelism)
        {
            var userKrSets = _keywordResponseSetDac
                .GetAll()
                .GroupBy(s => s.UserId)
                .Where(ShouldProcessUserSets);

            if(shouldParallelize)
                Parallel.ForEach(userKrSets, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, ProcessUserSets);
            else
            {
                foreach(var grouping in userKrSets)
                    ProcessUserSets(grouping);
            }
        }

        private bool ShouldProcessUserSets(IGrouping<int, KeywordResponseSet> grouping)
        {
            var userId = grouping.Key;

            var redditUser = _redditUserDac.GetByUser(userId)?.FirstOrDefault();

            if (redditUser == null)
                _logger.Info($"UserId: {userId} has no linked reddit account and will not be processed.");

            return redditUser != null;
        }

        private void ProcessUserSets(IGrouping<int, KeywordResponseSet> grouping)
        {
            var subredditDac = ServiceLocator.Current.GetInstance<ISubredditDac>();

            _logger.Info($"Processing sets for UserId: {grouping.Key}.");

            foreach (var set in grouping)
            {
                ProcessSet(grouping.Key, set, subredditDac);
            }
        }

        private void ProcessSet(int userId, KeywordResponseSet set, ISubredditDac subredditDac)
        {
            _logger.Info($"Processing keyword [{set.Keyword}] for UserId: {userId}");

            var subreddits = subredditDac.GetByKeywordResponseSetId(set.Id)?.ToList() ?? new List<Subreddit>();

            if (subreddits.Count == 0)
            {
                _logger.Info($"Keyword [{set.Keyword}] for UserId: {userId} has no associated subreddits.");
                return;
            }

            // retrieve comments from subreddits (last N minutes or N posts)
            // repond to them
        }
    }
}

