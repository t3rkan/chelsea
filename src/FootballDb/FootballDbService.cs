using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using MongoDB.Driver;
using FootballApi;

namespace FootballDb
{
    public class FootballDbService
    {
        private const string ConnectionString = "mongodb://localhost:27017";
        private const string DatabaseName = "API-Football-Database";
        private const string CollectionName = "Responses";

        private readonly IMongoCollection<FootballDbModel> _responses;
        private readonly FootballService _footballService;

        public FootballDbService(FootballService footballService)
        {
            var client = new MongoClient(ConnectionString);

            var database = client.GetDatabase(DatabaseName);

            _responses = database.GetCollection<FootballDbModel>(CollectionName);

            _footballService = footballService;
        }

        public long Count() => _responses.EstimatedDocumentCount();

        private void InsertMany(IEnumerable<FootballDbModel> responses) =>
            _responses.InsertMany(responses);

        public DeleteResult DeleteAll() => _responses.DeleteMany(x => true);

        public List<FootballDbModel> Get() => _responses.Find(x => true).ToList();

        public async Task SeedAsync()
        {
            if(this.Count() == 0)
            {
                var responses = await this.GetResponses();

                this.InsertMany(responses);
            }
        }

        private async Task<IEnumerable<FootballDbModel>> GetResponses()
        {
            var footballResponses = new List<FootballResponse>();

            var leagues = await _footballService.GetLeaguesAsync(49, 2019);
            var standings = await _footballService.GetStandingsAsync(49, 2019);
            var fixtures = await _footballService.GetFixturesAsync(49, 2019);
            var players = await _footballService.GetPlayersStatisticsAsync(49, 2019);

            footballResponses = footballResponses
                .Concat(leagues)
                .Concat(standings)
                .Concat(fixtures)
                .Concat(players)
                .ToList();

            var dbModels = footballResponses
                .Select(x => this.MapResponse(x));

            return dbModels;
        }

        // TODO: Content should not be string
        private FootballDbModel MapResponse(FootballResponse res) =>
            new FootballDbModel
            {
                Endpoint = res.Get.ToString(),
                Parameters = JsonSerializer
                    .Deserialize<Dictionary<string, string>>(
                            res.Parameters.ToString()),
                Results = res.Results.GetInt32(),
                Paging = JsonSerializer
                    .Deserialize<Dictionary<string, int>>(
                            res.Paging.ToString()),
                Content = res.Response.ToString(),
                RequestsRemaining = res.RequestsRemaining
            };
    }
}
