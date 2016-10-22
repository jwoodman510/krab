using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Krab.DataAccess.Dac;
using Krab.DataAccess.Subreddit;
using Krab.Web.Controllers.Api;
using Krab.Web.Models.Response;

namespace Krab.Web.Controllers
{
    public class SubredditsController : BaseController
    {
        private readonly ISubredditDac _subredditDac;
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;

        public SubredditsController(ISubredditDac subredditDac, IKeywordResponseSetDac keywordResponseSetDac)
        {
            _subredditDac = subredditDac;
            _keywordResponseSetDac = keywordResponseSetDac;
        }

        [HttpGet]
        public OkResponse<IList<Subreddit>> Get(int keywordResponseSetId)
        {
            var subreddits = _subredditDac.GetByKeywordResponseSetId(keywordResponseSetId)
                ?.ToList() ?? new List<Subreddit>();

            return new OkResponse<IList<Subreddit>>(subreddits);
        }

        [HttpDelete]
        public OkResponse DeleteSubreddit(int subredditId)
        {
            var subreddit = _subredditDac.Get(subredditId);
            
            if(subreddit == null)
                return new OkResponse();

            if(_keywordResponseSetDac.Get(subreddit.KeywordResponseSetId).UserId != GetUserId())
                throw new HttpException((int)HttpStatusCode.Unauthorized, "User is unauthorized.");

            _subredditDac.Delete(new List<int> {subredditId});

            return new OkResponse();
        }

        [HttpPost]
        public OkResponse<Subreddit> Create([FromBody] Subreddit subreddit)
        {
            var created = _subredditDac.Insert(subreddit);

            return new OkResponse<Subreddit>(created);
        }
    }
}