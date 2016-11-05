using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Krab.DataAccess.Exception;
using Krab.DataAccess.KeywordResponseSet;
using Krab.DataAccess.Subreddit;

namespace Krab.DataAccess.Dac
{
    public interface IKeywordResponseSetDac
    {
        IQueryable<KeywordResponseSet.KeywordResponseSet> GetAll();

        IEnumerable<KeywordResponseSet.KeywordResponseSet> GetByUserId(int userId);

        KeywordResponseSet.KeywordResponseSet Get(int id);

        KeywordResponseSet.KeywordResponseSet Insert(KeywordResponseSet.KeywordResponseSet set);

        KeywordResponseSet.KeywordResponseSet Update(KeywordResponseSet.KeywordResponseSet set);

        void Delete(IEnumerable<int> ids);
    }

    public class KeywordResponseSetDac : IKeywordResponseSetDac
    {
        private readonly KeywordResponseSetsDb _keywordResponseSetsDb;
        private readonly SubredditDb _subredditDb;

        public KeywordResponseSetDac(KeywordResponseSetsDb keywordResponseSetsDb, SubredditDb subredditDb)
        {
            _keywordResponseSetsDb = keywordResponseSetsDb;
            _subredditDb = subredditDb;
        }

        public IQueryable<KeywordResponseSet.KeywordResponseSet> GetAll()
        {
            return _keywordResponseSetsDb.KeywordResponseSets
                .AsNoTracking();
        }

        public IEnumerable<KeywordResponseSet.KeywordResponseSet> GetByUserId(int userId)
        {
            return _keywordResponseSetsDb.KeywordResponseSets
                .AsNoTracking()
                .Where(responseSet => responseSet.UserId == userId);
        }

        public KeywordResponseSet.KeywordResponseSet Get(int id)
        {
            return _keywordResponseSetsDb.KeywordResponseSets.Find(id);
        }

        public KeywordResponseSet.KeywordResponseSet Insert(KeywordResponseSet.KeywordResponseSet set)
        {
            set.Id = 0;

            if (string.IsNullOrWhiteSpace(set.Keyword))
                throw new ValidationException("Missing Keyword.");

            if(set.Keyword.Length > 50 || set.Keyword.Length < 5)
                throw new ValidationException("Keyword length must be 5 - 50 characters.");

            if (string.IsNullOrWhiteSpace(set.Response))
                throw new ValidationException("Missing Response.");

            if (set.Response.Length > 1000 || set.Response.Length < 1)
                throw new ValidationException("Response length must be 1 - 1000 characters.");

            if (set.StatusId != (int)KeywordResponseSetStatus.Active && set.StatusId != (int)KeywordResponseSetStatus.Paused)
                throw new ValidationException("Invalid Status.");
            
            if (KeywordExists(set.Keyword, set.UserId))
                throw new ValidationException("Keyword already exists.");

            if(GetByUserId(set.UserId)?.Count() >= 10)
                throw new ValidationException("Maximum count reached.");

            _keywordResponseSetsDb.KeywordResponseSets.Add(set);
            _keywordResponseSetsDb.SaveChanges();

            return set;
        }

        public KeywordResponseSet.KeywordResponseSet Update(KeywordResponseSet.KeywordResponseSet set)
        {
            if(set.Id < 1)
                throw new ValidationException("Invalid KeywordResponseSet Id.");

            if (string.IsNullOrWhiteSpace(set.Keyword))
                throw new ValidationException("Missing Keyword.");

            if (string.IsNullOrWhiteSpace(set.Response))
                throw new ValidationException("Missing Response.");

            if(set.StatusId != (int)KeywordResponseSetStatus.Active && set.StatusId != (int)KeywordResponseSetStatus.Paused)
                throw new ValidationException("Invalid Status.");

            var previous = Get(set.Id);

            if(previous == null)
                throw new NotFoundException("Not Found.");

            if(KeywordExists(set.Keyword, set.UserId))
                throw new ValidationException("Keyword already exists.");

            var entry = _keywordResponseSetsDb.Entry(previous);

            entry.Entity.Keyword = set.Keyword;
            entry.Entity.Response = set.Response;
            entry.Entity.StatusId = set.StatusId;
            entry.Entity.StatusId = set.StatusId;

            _keywordResponseSetsDb.SaveChanges();

            _keywordResponseSetsDb.Entry(previous).State = EntityState.Detached;


            return set;
        }

        public void Delete(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var existing = _keywordResponseSetsDb.KeywordResponseSets.Find(id);

                if (existing == null)
                    return;

                _subredditDb.Subreddits.RemoveRange(_subredditDb.Subreddits.Where(s => s.KeywordResponseSetId == id));
                _subredditDb.SaveChanges();

                var entry = _keywordResponseSetsDb.Entry(existing);
                entry.State = EntityState.Deleted;

                _keywordResponseSetsDb.SaveChanges();
            }
        }

        private bool KeywordExists(string keyword, int userId)
        {
            return _keywordResponseSetsDb
                .KeywordResponseSets
                .Any(s => s.Keyword.ToLower() == keyword && s.UserId == userId);
        }
    }
}
