namespace Krab.Web.Models
{
    public class RedditUser
    {
        public string UserName { get; set; }

        public int Id { get; set; }

        public RedditUser() { }

        public RedditUser(DataAccess.RedditUser.RedditUser user)
        {
            UserName = user.UserName;
            Id = user.Id;
        }
    }
}