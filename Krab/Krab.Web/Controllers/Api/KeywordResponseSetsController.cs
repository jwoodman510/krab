using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
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

        [HttpDelete]
        public OkResponse DeleteKeywordResponseSets(int keywordResponseSetId)
        {
            var set = _keywordResponseSetDac.Get(keywordResponseSetId);

            if (set == null)
                return new OkResponse();

            if (set.UserId != GetUserId())
                throw new HttpException((int)HttpStatusCode.Unauthorized, "User is unauthorized.");
            
            _keywordResponseSetDac.Delete(new List<int> { set.Id });

            return new OkResponse();
        }

        [HttpPut]
        public OkResponse<KeywordResponseSet> Update([FromBody] KeywordResponseSet set)
        {
            var setDb = _keywordResponseSetDac.Get(set.Id);

            if (setDb == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid KeywordResponseSet.");

            if (set.UserId != GetUserId())
                throw new HttpException((int)HttpStatusCode.Unauthorized, "User is unauthorized.");

            _keywordResponseSetDac.Update(set);

            return new OkResponse<KeywordResponseSet>(set);
        }

        [HttpPost]
        public OkResponse<KeywordResponseSet> Create([FromBody] KeywordResponseSet set)
        {
            set.Id = 0;
            set.UserId = GetUserId();

            var created = _keywordResponseSetDac.Insert(set);

            return new OkResponse<KeywordResponseSet>(created);
        } 
    }
}