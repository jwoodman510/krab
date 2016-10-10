using Krab.DataAccess.Dac;
using Krab.DataAccess.KeywordResponseSet;
using Krab.DataAccess.RedditUser;
using Krab.DataAccess.Subreddit;
using Krab.DataAccess.User;
using Microsoft.Practices.Unity;

namespace Krab.DataAccess
{
    public static class Configuration
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<UserDb, UserDb>();
            container.RegisterType<KeywordResponseSetsDb, KeywordResponseSetsDb>();
            container.RegisterType<RedditUserDb, RedditUserDb>();
            container.RegisterType<SubredditDb, SubredditDb>();

            container.RegisterType<IUserDac, UserDac>();
            container.RegisterType<IKeywordResponseSetDac, KeywordResponseSetDac>();
            container.RegisterType<IRedditUserDac,RedditUserDac>();
            container.RegisterType<ISubredditDac, SubredditDac>();
        }
    }
}
