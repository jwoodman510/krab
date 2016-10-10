using System.Collections.Generic;
using System.Linq;
using Krab.DataAccess.Subreddit;

namespace Krab.DataAccess.Dac
{
    public interface ISubredditDac
    {
        IEnumerable<Subreddit.Subreddit> GetByKeywordResponseSetId(int id);
    }

    public class SubredditDac : ISubredditDac
    {
        private readonly SubredditDb _subredditDb;

        public SubredditDac(SubredditDb subredditDb)
        {
            _subredditDb = subredditDb;
        }

        public IEnumerable<Subreddit.Subreddit> GetByKeywordResponseSetId(int id)
        {
            return _subredditDb.Subreddits
                .AsNoTracking()
                .Where(s => s.KeywordResponseSetId == id);
        }
    }
}
