using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RyaUploader.Core.Facade;
using RyaUploader.Core.Models;
using Serilog;

namespace RyaUploader.Core.Services.SteamServices
{
    public class SteamApi : ISteamApi
    {
        private readonly IHttpClient _client;
        // TODO: change this to something else when commiting
        private const string KEY = "3C97DE56AF018917F183FB805DA6B17B";
        
        public SteamApi(IHttpClient client)
        {
            _client = client;
        }

        public async Task<Dictionary<long, PlayerProfile>> GetPlayerProfilesAsync(long[] ids)
        {
            Log.Information($"Getting the steamProfiles for {ids}");


            var response = await _client.GetStringAsync(
                $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={KEY}&steamids={string.Join(",", ids)}");
            var apiresponse = JObject.Parse(response);

            var playerProfiles = apiresponse["response"]["players"].Children().Values<PlayerProfile>().ToArray();

            var result = new Dictionary<long, PlayerProfile>();

            for (var i = 0; i < 10; i++)
            {
                result.Add(playerProfiles[i].SteamId, playerProfiles[i]);
            }

            return result;
        }
    }
}
