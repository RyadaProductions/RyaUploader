using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RyaUploader.Core.Facade;
using RyaUploader.Core.Models;
using Serilog;

namespace RyaUploader.Core.Services.SteamServices
{
    public class SteamApi : ISteamApi
    {
        private readonly IHttpClient _client;
        // TODO: Change this key to be invalid
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

            // Use anonymous type to unwrap the playerprofiles json
            var anonymousDefinition = new
            {
                response = new
                {
                    players = new List<PlayerProfile>()
                }
            };

            var deserializedAnonymousType = JsonConvert.DeserializeAnonymousType(response, anonymousDefinition);

            return deserializedAnonymousType.response.players.ToDictionary(x => x.SteamId);
        }
    }
}
