using Krab.Api.Apis;
using Krab.Bus;
using Krab.DataAccess.Dac;
using Krab.Logger;
using Krab.Messages;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Krab.KeywordResponseSetProcessorService.Subscribers
{
    public class ProcessKeywordResponseSetSubscriber : IMessageSubscriber<ProcessKeywordResponseSet>
    {
        private readonly ILogger _logger;
        private readonly IAuthApi _authApi;
        private readonly ISubredditDac _subRedditDac;
        private readonly ISendBus _sendBus;

        public ProcessKeywordResponseSetSubscriber(ILogger logger, IAuthApi authApi, ISubredditDac subredditDac, ISendBus sendBus)
        {
            _logger = logger;
            _authApi = authApi;
            _subRedditDac = subredditDac;
            _sendBus = sendBus;
        }

        public void Receive(ProcessKeywordResponseSet message)
        {
            _logger.LogInfo($"Processing keyword [{message.Keyword}] for UserId: {message.UserId}");

            var subreddits = _subRedditDac.GetByKeywordResponseSetId(message.Id)?.ToList() ??
                             new List<DataAccess.Subreddit.Subreddit>();

            if (subreddits.Count == 0)
            {
                _logger.LogInfo(
                    $"Keyword [{message.Keyword}] for UserId: {message.UserId} has no associated subreddits.");
                return;
            }

            if (subreddits.Count > 5)
            {
                _logger.LogInfo(
                    $"Keyword [{message.Keyword}] for UserId: {message.UserId} has {subreddits.Count} associated subreddits. Truncating the list.");
                subreddits = subreddits.GetRange(0, 5);
            }

            var accessToken = _authApi.GetAccessToken(message.RedditUserId);

            var reddit = new Reddit(accessToken);

            foreach (var subreddit in subreddits)
            {
                _logger.LogInfo($"Retreiving the last 100 comments from /r/{subreddit.SubredditName}...");

                Subreddit s = null;

                try
                {
                    s = reddit.GetSubreddit(subreddit.SubredditName);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to get subreddit: {subreddit.SubredditName}", ex);
                }

                var comments = s
                    ?.Comments
                    ?.GetListing(100)
                    ?.ToList() ?? new List<Comment>();

                var numResponses = 0;

                foreach (var comment in comments)
                {
                    if (HasSameAuthor(message.RedditUserName, comment))
                        continue;

                    if (DoesNotContainKeyword(message.Keyword, comment))
                        continue;

                    if (HasAlreadyReplied(message.RedditUserName, comments, comment))
                        continue;

                    _logger.LogInfo($"Keyword [{message.Keyword}] for UserId: {message.UserId} => Replying to commentId: {comment.Id}.");

                    try
                    {
                        comment.Reply(message.Response);

                        numResponses++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to reply comment. Id={comment.Id}.", ex);
                    }

                    if (numResponses > 0)
                    {
                        _sendBus.PublishAsync(new KeywordResponseSetResponsesSubmitted
                        {
                            KeywordResponseSetId = message.Id,
                            DateTimeUtc = DateTime.UtcNow,
                            SubredditId = subreddit.Id,
                            NumResponses = numResponses
                        });
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
