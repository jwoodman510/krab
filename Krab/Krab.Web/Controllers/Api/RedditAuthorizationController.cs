using System;
using System.Linq;
using System.Web.Http;
using Krab.Caching;
using Krab.DataAccess.Dac;
using Krab.Global;
using Krab.Web.Models.Response;
using Microsoft.AspNet.Identity;

namespace Krab.Web.Controllers.Api
{
    public class RedditAuthorizationController : BaseController
    {
        private readonly ICache _cache;
        private readonly IRedditUserDac _redditUserDac;

        public RedditAuthorizationController(ICache cache, IRedditUserDac redditUserDac)
        {
            _cache = cache;
            _redditUserDac = redditUserDac;
        }

        public OkResponse<string> Get()
        {
            var state = Guid.NewGuid().ToString();
            var userId = User.Identity.GetUserId();

            _cache.SetValue(
                CacheKeys.RedditAuthState(userId),
                state,
                60 * 5);

            var authUrl = "https://www.reddit.com/api/v1/authorize" +
                          $"?client_id={AppSettings.ClientId}" +
                          "&response_type=code" +
                          $"&state={state}" +
                          $"&redirect_uri={AppSettings.RedirectUri}" +
                          "&duration=permanent" +
                          "&scope=identity read submit";

            return new OkResponse<string>(authUrl);
        }

        [HttpDelete]
        public OkResponse UnlinkRedditAccount()
        {
            var redditAcct = _redditUserDac.GetByUser(GetUserId()).FirstOrDefault();

            if (redditAcct != null)
                _redditUserDac.Delete(redditAcct.Id);

            return new OkResponse();
        } 
    }
}