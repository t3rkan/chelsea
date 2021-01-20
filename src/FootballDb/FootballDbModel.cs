using System.Collections.Generic;

namespace FootballDb
{
    public class FootballDbModel
    {
        public string Endpoint { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public int Results { get; set; }
        public Dictionary<string, int> Paging { get; set; }
        public string Content { get; set; }
        public int? RequestsRemaining { get; set; }
    }
}
