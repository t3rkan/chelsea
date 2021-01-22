using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using FootballApi;

namespace FootballDb
{
    public class FootballDbService
    {
        private const string ConnectionString = "mongodb://localhost:27017";
        private const string DatabaseName = "API-Football-Database";
        private const string CollectionName = "Models";

        private readonly IMongoCollection<FootballDbModel> _dbModels;
        private readonly FootballApiService _apiService;

        public FootballDbService(FootballApiService apiService)
        {
            var client = new MongoClient(ConnectionString);

            var database = client.GetDatabase(DatabaseName);

            _dbModels = database.GetCollection<FootballDbModel>(CollectionName);

            _apiService = apiService;
        }

        public long Count() => _dbModels.EstimatedDocumentCount();

        private void InsertMany(IEnumerable<FootballDbModel> dbModels) =>
            _dbModels.InsertMany(dbModels);

        public DeleteResult DeleteAll() => _dbModels.DeleteMany(x => true);

        public List<FootballDbModel> Get() => _dbModels.Find(x => true).ToList();

        public FootballDbModel Get(
                string endpoint, int team, int season, int page)
        {
            var filter = Builders<FootballDbModel>.Filter
                .Eq("Endpoint", endpoint);
            var projection = Builders<FootballDbModel>.Projection
                .Exclude("_id");

            var model = _dbModels
                .Find<FootballDbModel>(filter)
                .Project<FootballDbModel>(projection)
                .ToList()
                .FirstOrDefault(x =>
                        x.Parameters["team"] == team.ToString() &&
                        x.Parameters["season"] == season.ToString() &&
                        x.Paging["current"] == page);

            return model;
        }

        public async Task SeedAsync()
        {
            if(this.Count() == 0)
            {
                var dbModels = await this.GetDbModels();

                this.InsertMany(dbModels);
            }
        }

        private async Task<IEnumerable<FootballDbModel>> GetDbModels()
        {
            var apiModels = new List<FootballApiModel>();

            var leagues = await _apiService.GetLeaguesAsync(49, 2019);
            var standings = await _apiService.GetStandingsAsync(49, 2019);
            var fixtures = await _apiService.GetFixturesAsync(49, 2019);
            var players = await _apiService.GetPlayersStatisticsAsync(49, 2019);

            apiModels = apiModels
                .Concat(leagues)
                .Concat(standings)
                .Concat(fixtures)
                .Concat(players)
                .ToList();

            var dbModels = apiModels.Select(x => this.MapApiModels(x));

            return dbModels;
        }

        private FootballDbModel MapApiModels(FootballApiModel apiModel) =>
            new FootballDbModel
            {
                Endpoint = apiModel.Get.ToString(),
                Parameters = JsonSerializer
                    .Deserialize<Dictionary<string, string>>(
                            apiModel.Parameters.ToString()),
                Results = apiModel.Results.GetInt32(),
                Paging = JsonSerializer
                    .Deserialize<Dictionary<string, int>>(
                            apiModel.Paging.ToString()),
                Content = BsonSerializer
                    .Deserialize<BsonArray>(
                            apiModel.Response.ToString()),
                RequestsRemaining = apiModel.RequestsRemaining
            };
    }
}
