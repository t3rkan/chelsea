using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FootballDb;
using FootballApi;

namespace ChelseaApi
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly FootballDbService _db;

        public HomeController()
        {
            var api = new FootballApiService("");
            _db = new FootballDbService(api);
        }

        [HttpGet("{endpoint}")]
        public ActionResult<object> Get(
                string endpoint, int team, int season, int page = 1)
        {
            var leagues = _db.Get(endpoint, team, season, page);

            return new
            {
                Endpoint = leagues.Endpoint,
                Parameters = leagues.Parameters,
                Results = leagues.Results,
                Paging = leagues.Paging,
                Content = JsonSerializer.Deserialize<JsonElement>(
                        leagues.Content.ToString()),
                RequestsRemaining = leagues.RequestsRemaining
            };
        }
    }
}
