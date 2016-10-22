using Krab.Api.Apis;
using Krab.Bus;
using Krab.DataAccess.Dac;
using Krab.Messages;
using log4net;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Krab.KeywordResponseSetProcessorService.Subscribers
{
    public class ProcessKeywordResponseSetSubscriber : IMessageSubscriber<ProcessKeywordResponseSet>
    {
        private readonly ILog _logger;
        private readonly IAuthApi _authApi;
        private readonly ISubredditDac _subRedditDac;

        public ProcessKeywordResponseSetSubscriber(ILog logger, IAuthApi authApi, ISubredditDac subredditDac)
        {
            _logger = logger;
            _authApi = authApi;
            _subRedditDac = subredditDac;
        }

        public void Receive(ProcessKeywordResponseSet message)
        {
            _logger.Info($"{message.GetType()} Received. KeywordResponseSetId={message.Id}.");

            Process(message);

            _logger.Info($"{message.GetType()} Complete. KeywordResponseSetId={message.Id}.");
        }

        public void Process(ProcessKeywordResponseSet message)
        {
            _logger.Info($"Processing keyword [{message.Keyword}] for UserId: {message.UserId}");

            var subreddits = _subRedditDac.GetByKeywordResponseSetId(message.Id)?.ToList() ?? new List<DataAccess.Subreddit.Subreddit>();

            if (subreddits.Count == 0)
            {
                _logger.Info($"Keyword [{message.Keyword}] for UserId: {message.UserId} has no associated subreddits.");
                return;
            }

            if (subreddits.Count > 5)
            {
                _logger.Info($"Keyword [{message.Keyword}] for UserId: {message.UserId} has {subreddits.Count} associated subreddits. Truncating the list.");
                subreddits = subreddits.GetRange(0, 5);
            }

            var accessToken = _authApi.GetAccessToken(message.RedditUserId);

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
                    if (HasSameAuthor(message.RedditUserName, comment))
                        continue;

                    if (DoesNotContainKeyword(message.Keyword, comment))
                        continue;

                    if (HasAlreadyReplied(message.RedditUserName, comments, comment))
                        continue;

                    _logger.Info($"Keyword [{message.Keyword}] for UserId: {message.UserId} => Replying to commentId: {comment.Id}.");

                    try
                    {
                        comment.Reply(message.Response);
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

            return childrenOfThisComment.Any(c => HasSameAuthor(redditUserName, c));
        }

        private static bool DoesNotContainKeyword(string keyword, Comment comment)
        {
            return comment.Body?.ToLower() != keyword?.ToLower();
        }

        private static bool HasSameAuthor(string redditUserName, Comment comment)
        {
            return comment.Author?.ToLower() == redditUserName?.ToLower();
        }
    }
}
