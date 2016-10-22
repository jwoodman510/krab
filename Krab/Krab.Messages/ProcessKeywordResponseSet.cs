using Krab.Bus;

namespace Krab.Messages
{
    public class ProcessKeywordResponseSet : Message
    {
        public int UserId { get; set; }

        public int RedditUserId { get; set; }

        public string RedditUserName { get; set; }

        public int Id { get; set; }

        public int StatusId { get; set; }

        public string Keyword { get; set; }

        public string Response { get; set; }
    }
}
