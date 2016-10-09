using System.Collections.Generic;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.Web.Models.Response;

namespace Krab.Web.Controllers.Api
{
    public class KeywordResponseSetsController : BaseController
    {
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;

        public KeywordResponseSetsController(IKeywordResponseSetDac keywordResponseSetDac)
        {
            _keywordResponseSetDac = keywordResponseSetDac;
        }
        
        public OkResponse<IEnumerable<KeywordResponseSet>> GetKeywordResponseSets()
        {
            var sets = _keywordResponseSetDac.GetByUserId(GetUserId());
            return new OkResponse<IEnumerable<KeywordResponseSet>>(sets);
        }
    }
}