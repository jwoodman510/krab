using System.Collections.Generic;
using System.Linq;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.Web.Models.Response;
using System.Web.Http;

namespace Krab.Web.Controllers.Api
{
    public class KeywordResponseSetsController : BaseController
    {
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;

        public KeywordResponseSetsController(IKeywordResponseSetDac keywordResponseSetDac)
        {
            _keywordResponseSetDac = keywordResponseSetDac;
        }
        [HttpGet]
        public OkResponse<IList<KeywordResponseSet>> Get()
        {
            var sets = _keywordResponseSetDac.GetByUserId(GetUserId());
            return new OkResponse<IList<KeywordResponseSet>>(sets?.ToList() ?? new List<KeywordResponseSet>());
        }
        
    }
}