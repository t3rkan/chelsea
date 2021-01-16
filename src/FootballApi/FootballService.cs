using System.Threading.Tasks;

namespace FootballApi
{
    public sealed class FootballService
    {
        private FootballClient _client;

        public FootballService(string authToken)
        {
            _client = new FootballClient(authToken);
        }

        // Leagues in which the {team} has played at least one match in {season}
        public async Task<FootballResponse> GetLeaguesAsync(int teamId, int season)
        {
            var response = await _client.GetAsync(
                    $"/leagues?team={teamId}&season={season}");

            return response;
        }


        // Standings from one {league} & {season}
        public async Task<FootballResponse> GetStandingsAsync(
                int leagueId, int season)
        {
            var response = await _client.GetAsync(
                    $"/standings?league={leagueId}&season={season}");

            return response;
        }

        // Fixtures for a {team} in a {season}
        public async Task<FootballResponse> GetFixturesAsync(int teamId, int season)
        {
            var response = await _client.GetAsync(
                    $"/fixtures?team={teamId}&season={season}");

            return response;
        }

        // Player statistics from one {team} & {season}
        public async Task<FootballResponse> GetPlayersStatisticsAsync(
                int teamId, int season)
        {
            var response = await _client.GetAsync(
                    $"/players?team={teamId}&season={season}");

            return response;
        }
    }
}
