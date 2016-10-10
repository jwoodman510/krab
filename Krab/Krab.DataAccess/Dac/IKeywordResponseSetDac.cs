using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krab.DataAccess.KeywordResponseSet;

namespace Krab.DataAccess.Dac
{
    public interface IKeywordResponseSetDac
    {
        IEnumerable<KeywordResponseSet.KeywordResponseSet> GetAll();
        IEnumerable<KeywordResponseSet.KeywordResponseSet> GetByUserId(int userId);
        KeywordResponseSet.KeywordResponseSet Get(int id);
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
    }
}
