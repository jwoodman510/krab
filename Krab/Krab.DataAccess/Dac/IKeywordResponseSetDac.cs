using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Krab.DataAccess.Exception;
using Krab.DataAccess.KeywordResponseSet;

namespace Krab.DataAccess.Dac
{
    public interface IKeywordResponseSetDac
    {
        IEnumerable<KeywordResponseSet.KeywordResponseSet> GetAll();
        IEnumerable<KeywordResponseSet.KeywordResponseSet> GetByUserId(int userId);
        KeywordResponseSet.KeywordResponseSet Get(int id);

        KeywordResponseSet.KeywordResponseSet Update(KeywordResponseSet.KeywordResponseSet set);

        void Delete(IEnumerable<int> ids);
    }

    public class KeywordResponseSetDac : IKeywordResponseSetDac
    {
        private readonly KeywordResponseSetsDb _keywordResponseSetsDb;

        public KeywordResponseSetDac(KeywordResponseSetsDb keywordResponseSetsDb)
        {
            _keywordResponseSetsDb = keywordResponseSetsDb;
        }

        public IEnumerable<KeywordResponseSet.KeywordResponseSet> GetAll()
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
        
        public KeywordResponseSet.KeywordResponseSet Update(KeywordResponseSet.KeywordResponseSet set)
        {
            if(set.Id < 1)
                throw new ValidationException("Invalid KeywordResponseSet Id.");

            if (string.IsNullOrWhiteSpace(set.Keyword))
                throw new ValidationException("Missing Keyword.");

            if (string.IsNullOrWhiteSpace(set.Status))
                throw new ValidationException("Missing Response.");

            if(set.StatusId != (int)KeywordResponseSetStatus.Active && set.StatusId != (int)KeywordResponseSetStatus.Paused)
                throw new ValidationException("Invalid Status.");

            var previous = Get(set.Id);

            if(previous == null)
                throw new NotFoundException("Not Found.");

            var entry = _keywordResponseSetsDb.Entry(previous);

            entry.Entity.Keyword = set.Keyword;
            entry.Entity.Response = set.Response;
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

                var entry = _keywordResponseSetsDb.Entry(existing);
                entry.State = EntityState.Deleted;

                _keywordResponseSetsDb.SaveChanges();
            }
        }
    }
}
