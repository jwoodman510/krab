using System.Data.Entity;
using System.Linq;
using Krab.DataAccess.RedditUser;
using Krab.DataAccess.Exception;

namespace Krab.DataAccess.Dac
{
    public interface IRedditUserDac
    {
        RedditUser.RedditUser Get(int id);
        IQueryable<RedditUser.RedditUser> GetByUser(int userId);

        RedditUser.RedditUser Create(RedditUser.RedditUser u);

        void UpdateAccessToken(int id, string accessToken);
    }

    public class RedditUserDac : IRedditUserDac
    {
        private readonly RedditUserDb _context;

        public RedditUserDac(RedditUserDb context)
        {
            _context = context;
        }

        public RedditUser.RedditUser Get(int id)
        {
            return _context
                .RedditUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id);
        }

        public IQueryable<RedditUser.RedditUser> GetByUser(int userId)
        {
            return _context
                .RedditUsers
                .AsNoTracking()
                .Where(u => u.UserId == userId);
        }
        
        public RedditUser.RedditUser Create(RedditUser.RedditUser u)
        {
            if (string.IsNullOrWhiteSpace(u.UserName))
                return null;

            if (string.IsNullOrWhiteSpace(u.AccessToken))
                return null;

            if (string.IsNullOrWhiteSpace(u.RefreshToken))
                return null;

            if (u.UserId < 1)
                return null;

            _context.RedditUsers.Add(u);
            _context.SaveChanges();

            return u;
        }

        public void UpdateAccessToken(int id, string accessToken)
        {
            if (id < 1)
                throw new ValidationException("Invalid RedditUser Id.");

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ValidationException("Missing access token.");

            var previous = _context.RedditUsers.Find(id);

            if (previous == null)
                throw new NotFoundException($"RedditUser {id} not found.");

            var entry = _context.Entry(previous);

            entry.Entity.AccessToken = accessToken;

            _context.SaveChanges();

            _context.Entry(previous).State = EntityState.Detached;
        }
    }
}
