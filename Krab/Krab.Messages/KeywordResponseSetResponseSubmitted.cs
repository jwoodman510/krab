using System;
using Krab.Bus;

namespace Krab.Messages
{
    public class KeywordResponseSetResponseSubmitted : Message
    {
        public int KeywordResponseSetId { get; set; }
        public long SubredditId { get; set; }
        public DateTime DateTimeUtc { get; set; }
    }
}
