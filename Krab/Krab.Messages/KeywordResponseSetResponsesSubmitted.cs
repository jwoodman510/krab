using System;
using Krab.Bus;

namespace Krab.Messages
{
    public class KeywordResponseSetResponsesSubmitted : Message
    {
        public int KeywordResponseSetId { get; set; }
        public long SubredditId { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public int NumResponses { get; set; }
    }
}
