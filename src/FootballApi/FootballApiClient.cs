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
    internal class FootballApiClient
    {
        private const string RequestsRemainingHeaderKey =
            "x-ratelimit-requests-remaining";
        private const string FootballApiUrl =
            "https://v3.football.api-sports.io";

        private HttpClient _client;

        public FootballApiClient(string authToken)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(FootballApiUrl);
            _client.DefaultRequestHeaders.Add("X-RapidAPI-Key", authToken);
        }

        public async Task<List<FootballApiModel>> GetAsync(string path)
        {
            var apiModels = new List<FootballApiModel>();

            try
            {
                var currentPage = 1;
                var pageParam = "";

                while(true)
                {
                    var httpRes = await _client.GetAsync(path + pageParam);

                    if(httpRes.StatusCode == HttpStatusCode.OK)
                    {
                        var apiModel = await this.MapResponseAsync(httpRes);

                        if(!this.HasErrors(apiModel))
                        {
                            this.MapHeaders(httpRes, apiModel);

                            apiModels.Add(apiModel);

                            if(currentPage != this.GetTotalPages(apiModel))
                            {
                                currentPage++;
                                pageParam = $"&page={currentPage}";
                            }
                            else break;
                        }
                        else throw new Exception(
                                await httpRes.Content.ReadAsStringAsync());
                    }
                    else throw new Exception(
                            await httpRes.Content.ReadAsStringAsync());
                }
            }
            catch
            {
                throw;
            }

            return apiModels;
        }

        private int GetTotalPages(FootballApiModel apiModel)
        {
            var paging = JsonSerializer
                .Deserialize<Dictionary<string, int>>(apiModel.Paging.ToString());

            return paging["total"];
        }

        private bool HasErrors(FootballApiModel apiModel)
        {
            return !(apiModel.Errors.ValueKind == JsonValueKind.Array);
        }

        private void MapHeaders(
                HttpResponseMessage httpRes,
                FootballApiModel apiModel)
        {
            apiModel.RequestsRemaining = int.Parse(httpRes.Headers
                    .GetValues(RequestsRemainingHeaderKey)
                    .FirstOrDefault());
        }

        private async Task<FootballApiModel> MapResponseAsync(
                HttpResponseMessage httpRes)
        {
            var options = new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true };

            var httpResStr = await httpRes.Content.ReadAsStringAsync();

            var apiModel = JsonSerializer.Deserialize<FootballApiModel>(
                    httpResStr, options);

            return apiModel;
        }
    }
}
