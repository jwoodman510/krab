namespace Krab.DataAccess.User
{
    public class CachedUser
    {
        public string Id { get; set; }
        public int UserId { get; set; }

        public CachedUser(AspNetUser aspNetUser)
        {
            Id = aspNetUser.Id;
            UserId = aspNetUser.UserId;
        }

        public CachedUser()
        {
            
        }
    }
}
