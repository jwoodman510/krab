using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Krab.DataAccess.Dac;
using Krab.DataAccess.Subreddit;
using Krab.Global.Extensions;
using Krab.Web.Models.Response;

namespace Krab.Web.Controllers.Api
{
    public class SubredditController : BaseController
    {
        private readonly ISubredditDac _subredditDac;

        public SubredditController(ISubredditDac subredditDac)
        {
            _subredditDac = subredditDac;
        }

        [HttpGet]
        public OkResponse<IList<Subreddit>> GetByKeywordResponseSet(int keywordResponseSetId)
        {
            var subreddits = _subredditDac.GetByKeywordResponseSetId(keywordResponseSetId)
                ?.ToList() ?? new List<Subreddit>();

            return new OkResponse<IList<Subreddit>>(subreddits);
        }

        [HttpPost]
        public OkResponse<Subreddit> Create([FromBody] Subreddit subreddit)
        {
            var created = _subredditDac.Insert(subreddit);

            return new OkResponse<Subreddit>(created);
        }

        [HttpPost]
        public OkResponse<IList<Subreddit>> CreateMultiple([FromBody] IList<Subreddit> subreddits)
        {
            var created = new List<Subreddit>();

            subreddits?.ForEach(s =>
            {
                var newSubreddit = _subredditDac.Insert(s);
                created.Add(newSubreddit);
            });
            
            return new OkResponse<IList<Subreddit>>(created);
        }

        [HttpDelete]
        public OkResponse Delete(int subredditId)
        {
            _subredditDac.Delete(new List<int> { subredditId });

            return new OkResponse();
        }

        [HttpDelete]
        public OkResponse DeleteMultiple([FromBody] IList<int> subredditIds)
        {
            _subredditDac.Delete(subredditIds);

            return new OkResponse();
        }
    }
}