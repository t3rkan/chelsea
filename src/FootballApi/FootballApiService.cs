using System.Threading.Tasks;
using System.Collections.Generic;

namespace FootballApi
{
    public sealed class FootballApiService
    {
        private FootballApiClient _client;

        public FootballApiService(string authToken)
        {
            _client = new FootballApiClient(authToken);
        }

        // Leagues in which the {team} has played at least one match in {season}
        public async Task<List<FootballApiModel>> GetLeaguesAsync(
                int teamId, int season)
        {
            var apiModel = await _client.GetAsync(
                    $"/leagues?team={teamId}&season={season}");

            return apiModel;
        }


        // Standings from one {team} & {season}
        public async Task<List<FootballApiModel>> GetStandingsAsync(
                int teamId, int season)
        {
            var apiModel = await _client.GetAsync(
                    $"/standings?team={teamId}&season={season}");

            return apiModel;
        }

        // Fixtures for a {team} in a {season}
        public async Task<List<FootballApiModel>> GetFixturesAsync(
                int teamId, int season)
        {
            var apiModel = await _client.GetAsync(
                    $"/fixtures?team={teamId}&season={season}");

            return apiModel;
        }

        // Player statistics from one {team} & {season}
        public async Task<List<FootballApiModel>> GetPlayersStatisticsAsync(
                int teamId, int season)
        {
            var apiModel = await _client.GetAsync(
                    $"/players?team={teamId}&season={season}");

            return apiModel;
        }
    }
}
