using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krab.Api.Apis;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.Global;
using log4net;
using Microsoft.Practices.ServiceLocation;
using NCron;
using RedditSharp;
using RedditSharp.Things;
using Subreddit = Krab.DataAccess.Subreddit.Subreddit;

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
        private readonly IAuthApi _authApi;

        public ProcessKeywordResponseSets(
            ILog logger,
            IRedditUserDac redditUserDac,
            IKeywordResponseSetDac keywordResponseSetDac,
            IAppSettingProvider appSettingProvider,
            IAuthApi authApi)
        {
            _logger = logger;
            _redditUserDac = redditUserDac;
            _keywordResponseSetDac = keywordResponseSetDac;
            _appSettingProvider = appSettingProvider;
            _authApi = authApi;
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
                ProcessSet(grouping.Key, redditUser.Id, redditUser.UserName, set, subredditDac);
            }
        }

        private void ProcessSet(int userId, int redditUserId, string redditUserName, KeywordResponseSet set, ISubredditDac subredditDac)
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

            var accessToken = _authApi.GetAccessToken(redditUserId);

            var reddit = new Reddit(accessToken);

            foreach (var subreddit in subreddits)
            {
                _logger.Info($"Retreiving the last 100 comments from /r/{subreddit.SubredditName}...");
                
                var comments = reddit
                    .GetSubreddit(subreddit.SubredditName)
                    ?.Comments
                    ?.GetListing(100)
                    ?.ToList() ?? new List<Comment>();

                foreach (var comment in comments)
                {
                    if (HasSameAuthor(redditUserName, comment))
                        continue;
                    
                    if (DoesNotContainKeyword(set, comment))
                        continue;

                    if (HasAlreadyReplied(redditUserName, comments, comment))
                        continue;

                    _logger.Info($"Keyword [{set.Keyword}] for UserId: {userId} => Replying to commentId: {comment.Id}.");

                    try
                    {
                        comment.Reply(set.Response);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Failed to reply comment. Id={comment.Id}.", ex);
                    }
                }
            }
        }

        private static bool HasAlreadyReplied(string redditUserName, IEnumerable<Comment> comments, Comment comment)
        {
            var childrenOfThisComment = comments.Where(c => c.ParentId.Split('_').Last() == comment.Id);
            
            return childrenOfThisComment.Where(c => c.ParentId == comment.ParentId.Split('_').Last()).Any(c => HasSameAuthor(redditUserName, c));
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

