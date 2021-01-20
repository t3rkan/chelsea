using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FootballApi
{
    internal class FootballClient
    {
        private const string RequestsRemainingHeaderKey =
            "x-ratelimit-requests-remaining";
        private const string FootballApiUrl =
            "https://v3.football.api-sports.io";

        private HttpClient _client;

        public FootballClient(string authToken)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(FootballApiUrl);
            _client.DefaultRequestHeaders.Add("X-RapidAPI-Key", authToken);
        }

        public async Task<List<FootballResponse>> GetAsync(string path)
        {
            List<FootballResponse> responses = new List<FootballResponse>();

            try
            {
                var currentPage = 1;
                var pageParam = "";

                while(true)
                {
                    var res = await _client.GetAsync(path + pageParam);

                    if(res.StatusCode == HttpStatusCode.OK)
                    {
                        var resAsString = await res.Content.ReadAsStringAsync();

                        var footballRes = this.MapResponse(resAsString);

                        if(!this.HasErrors(footballRes))
                        {
                            this.MapHeaders(res, footballRes);

                            responses.Add(footballRes);

                            if(currentPage != this.GetTotalPages(footballRes))
                            {
                                currentPage++;
                                pageParam = $"&page={currentPage}";
                            }
                            else break;
                        }
                        else throw new Exception(footballRes.ToString());
                    }
                    else throw new Exception(res.StatusCode.ToString());
                }
            }
            catch
            {
                throw;
            }

            return responses;
        }

        private int GetTotalPages(FootballResponse res)
        {
            var paging = JsonSerializer
                .Deserialize<Dictionary<string, int>>(res.Paging.ToString());

            return paging["total"];
        }

        private bool HasErrors(FootballResponse res)
        {
            return !(res.Errors.ValueKind == JsonValueKind.Array);
        }

        private void MapHeaders(
                HttpResponseMessage res,
                FootballResponse footballRes)
        {
            footballRes.RequestsRemaining = int.Parse(res.Headers
                    .GetValues(RequestsRemainingHeaderKey)
                    .FirstOrDefault());
        }

        private FootballResponse MapResponse(string res)
        {
            var options = new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true };

            var footballRes = JsonSerializer.Deserialize<FootballResponse>(
                    res, options);

            return footballRes;
        }
    }
}
