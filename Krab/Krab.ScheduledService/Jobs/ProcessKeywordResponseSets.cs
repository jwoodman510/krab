using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krab.Api.Apis;
using Krab.Api.ValueObjects;
using Krab.Api.ValueObjects.Comment;
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
        private readonly ICommentApi _commentApi;

        public ProcessKeywordResponseSets(
            ILog logger,
            IRedditUserDac redditUserDac,
            IKeywordResponseSetDac keywordResponseSetDac,
            IAppSettingProvider appSettingProvider,
            ICommentApi commentApi)
        {
            _logger = logger;
            _redditUserDac = redditUserDac;
            _keywordResponseSetDac = keywordResponseSetDac;
            _appSettingProvider = appSettingProvider;
            _commentApi = commentApi;
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

            var redditUser = _redditUserDac.GetByUser(grouping.Key).First();

            _logger.Info($"Processing sets for UserId: {grouping.Key}.");

            foreach (var set in grouping)
            {
                ProcessSet(grouping.Key, redditUser.UserName, set, subredditDac);
            }
        }

        private void ProcessSet(int userId, string redditUserName, KeywordResponseSet set, ISubredditDac subredditDac)
        {
            _logger.Info($"Processing keyword [{set.Keyword}] for UserId: {userId}");

            var subreddits = subredditDac.GetByKeywordResponseSetId(set.Id)?.ToList() ?? new List<Subreddit>();

            if (subreddits.Count == 0)
            {
                _logger.Info($"Keyword [{set.Keyword}] for UserId: {userId} has no associated subreddits.");
                return;
            }

            if (subreddits.Count > 5)
            {
                _logger.Info($"Keyword [{set.Keyword}] for UserId: {userId} has {subreddits.Count} associated subreddits. Truncating the list.");
                subreddits = subreddits.GetRange(0, 5);
            }

            foreach (var subreddit in subreddits)
            {
                _logger.Info($"Retreiving the last 100 comments from /r/{subreddit.SubredditName}...");

                var comments = _commentApi.GetNewBySubreddit(subreddit.SubredditName, 100);

                foreach (var comment in comments)
                {
                    if (HasSameAuthor(redditUserName, comment))
                        continue;
                    
                    if (DoesNotContainKeyword(set, comment))
                        continue;

                    if (HasAlreadyReplied(redditUserName, comments, comment))
                        continue;

                    _logger.Info($"Keyword [{set.Keyword}] for UserId: {userId} => Replying to commentId: {comment.Id}.");
                }
            }
        }

        private static bool HasAlreadyReplied(string redditUserName, IEnumerable<Comment> comments, Comment comment)
        {
            var childrenOfThisComment = comments.Where(c => c.ParentId == comment.Name);
            
            return childrenOfThisComment.Where(c => c.ParentId == comment.Name).Any(c => HasSameAuthor(redditUserName, c));
        }

        private static bool DoesNotContainKeyword(KeywordResponseSet set, Comment comment)
        {
            return comment.Body?.ToLower() != set.Keyword?.ToLower();
        }

        private static bool HasSameAuthor(string redditUserName, Comment comment)
        {
            return comment.Author?.ToLower() == redditUserName?.ToLower();
        }
    }
}

