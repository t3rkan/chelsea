using System.Collections.Generic;
using MongoDB.Bson;

namespace FootballDb
{
    public class FootballDbModel
    {
        public string Endpoint { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public int Results { get; set; }
        public Dictionary<string, int> Paging { get; set; }
        public BsonArray Content { get; set; }
        public int? RequestsRemaining { get; set; }
    }
}
