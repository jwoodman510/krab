using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Krab.DataAccess.Exception;
using Krab.DataAccess.Subreddit;

namespace Krab.DataAccess.Dac
{
    public interface ISubredditDac
    {
        Subreddit.Subreddit Get(int id);
        IEnumerable<Subreddit.Subreddit> GetByKeywordResponseSetId(int id);

        Subreddit.Subreddit Insert(Subreddit.Subreddit subreddit);

        void Delete(IEnumerable<int> ids);
    }

    public class SubredditDac : ISubredditDac
    {
        private readonly SubredditDb _subredditDb;
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;

        public SubredditDac(SubredditDb subredditDb, IKeywordResponseSetDac keywordResponseSetDac)
        {
            _subredditDb = subredditDb;
            _keywordResponseSetDac = keywordResponseSetDac;
        }

        public Subreddit.Subreddit Get(int id)
        {
            return _subredditDb
                .Subreddits
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<Subreddit.Subreddit> GetByKeywordResponseSetId(int id)
        {
            return _subredditDb.Subreddits
                .AsNoTracking()
                .Where(s => s.KeywordResponseSetId == id);
        }

        public Subreddit.Subreddit Insert(Subreddit.Subreddit subreddit)
        {
            subreddit.Id = 0;

            if(string.IsNullOrWhiteSpace(subreddit.SubredditName))
                throw new ValidationException("Missing subreddit name.");

            var set = _keywordResponseSetDac.Get(subreddit.KeywordResponseSetId);

            if(set == null)
                throw new KeyNotFoundException("Keyword-response set not found.");

            var subreddits = GetByKeywordResponseSetId(set.Id)
                ?.ToList() ?? new List<Subreddit.Subreddit>();

            if(subreddits.Count() >= 5)
                throw new ValidationException("Maximum subreddit count has been reached.");

            if(subreddits.Any(s => s.SubredditName?.ToLower() == subreddit.SubredditName.ToLower()))
                throw new ValidationException("Subreddit already exists.");

            _subredditDb.Subreddits.Add(subreddit);
            _subredditDb.SaveChanges();

            return subreddit;
        }

        public void Delete(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var existing = _subredditDb.Subreddits.Find(id);

                if (existing == null)
                    return;

                var entry = _subredditDb.Entry(existing);
                entry.State = EntityState.Deleted;

                _subredditDb.SaveChanges();
            }
        }
    }
}
