using System.Linq;
using Krab.DataAccess.Dac;
using Krab.Web.Models;
using Krab.Web.Models.Response;

namespace Krab.Web.Controllers.Api
{
    public class RedditUserController : BaseController
    {
        private readonly IRedditUserDac _redditUserDac;

        public RedditUserController(IRedditUserDac redditUserDac)
        {
            _redditUserDac = redditUserDac;
        }

        public OkResponse<RedditUser> Get()
        {
            var userDb = _redditUserDac.GetByUser(GetUserId())?.FirstOrDefault();

            return userDb == null
                ? new OkResponse<RedditUser>(new RedditUser())
                : new OkResponse<RedditUser>(new RedditUser(userDb));
        } 
    }
}