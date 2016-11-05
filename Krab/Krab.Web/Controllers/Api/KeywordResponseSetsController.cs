using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.Web.Models.Response;
using System.Web.Http;
using Krab.DataAccess.Subreddit;
using Krab.Global.Extensions;
using Krab.Web.Exceptions;

namespace Krab.Web.Controllers.Api
{
    public class KeywordResponseSetsController : BaseController
    {
        private readonly IKeywordResponseSetDac _keywordResponseSetDac;
        private readonly ISubredditDac _subredditDac;

        public KeywordResponseSetsController(IKeywordResponseSetDac keywordResponseSetDac, ISubredditDac subredditDac)
        {
            _keywordResponseSetDac = keywordResponseSetDac;
            _subredditDac = subredditDac;
        }

        [HttpGet]
        public OkResponse<IList<KeywordResponseSet>> Get()
        {
            var sets = _keywordResponseSetDac.GetByUserId(GetUserId());
            return new OkResponse<IList<KeywordResponseSet>>(sets?.ToList() ?? new List<KeywordResponseSet>());
        }

        [HttpDelete]
        public OkResponse DeleteKeywordResponseSet(int keywordResponseSetId)
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

            if (setDb.UserId != GetUserId())
                throw new HttpException((int)HttpStatusCode.Unauthorized, "User is unauthorized.");

           var updated =  _keywordResponseSetDac.Update(set);

            return new OkResponse<KeywordResponseSet>(updated);
        }

        [HttpPost]
        public OkResponse<KeywordResponseSet> Create([FromBody] KeywordResponseSet set)
        {
            set.UserId = GetUserId();
            var created = _keywordResponseSetDac.Insert(set);

            return new OkResponse<KeywordResponseSet>(created);
        }

        [HttpPut]
        public OkResponse<IList<Subreddit>> UpdateSubreddits([FromUri] int keywordResponseSetId, [FromBody] IEnumerable<string> subreddits)
        {
            var current = new List<Subreddit>();

            var set = _keywordResponseSetDac.Get(keywordResponseSetId);

            if(set?.UserId != GetUserId())
                throw new UnauthorizedAccessException();

            if (set == null)
                throw new NotFoundException($"Id: {keywordResponseSetId} not found.");

            var toUpdate = subreddits?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();

            var previous = _subredditDac.GetByKeywordResponseSetId(keywordResponseSetId)
                ?.ToDictionary(k => k.SubredditName.ToLower()) ?? new Dictionary<string, Subreddit>();

            foreach (var subreddit in previous)
            {
                if(toUpdate.All(u => u.ToLower() != subreddit.Key))
                    _subredditDac.Delete(new [] {subreddit.Value.Id});
            }
            
            foreach (var subreddit in toUpdate)
            {
                Subreddit existing;
                if (previous.TryGetValue(subreddit.ToLower(), out existing))
                {
                    current.Add(existing);
                }
                else
                {
                    var created = _subredditDac.Insert(new Subreddit
                    {
                        KeywordResponseSetId = keywordResponseSetId,
                        SubredditName = subreddit
                    });

                    current.Add(created);
                }
            }

            return new OkResponse<IList<Subreddit>>(current);
        }
    }
}