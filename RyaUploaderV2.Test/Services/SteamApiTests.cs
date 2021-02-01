using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RyaUploaderV2.Services.SteamServices;
using RyaUploaderV2.Test.Helpers;

namespace RyaUploaderV2.Test.Services
{
    [TestFixture]
    public class SteamApiTests
    {
        private string _key = "";

        [Test]
        public async Task GetPlayerProfile_ReturnsValidPersonModel()
        {
            var mockHandler = new Mock<HttpClientHandler>();

            var response = "{\"response\":{\"players\":[{\"steamid\":\"76561197960435530\",\"communityvisibilitystate\":3,\"profilestate\":1,\"personaname\":\"Robin\",\"lastlogoff\":1516019754,\"profileurl\":\"http://steamcommunity.com/id/robinwalker/\",\"avatar\":\"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4.jpg\",\"avatarmedium\":\"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4_medium.jpg\",\"avatarfull\":\"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4_full.jpg\",\"personastate\":0,\"realname\":\"Robin Walker\",\"primaryclanid\":\"103582791429521412\",\"timecreated\":1063407589,\"personastateflags\":0,\"loccountrycode\":\"US\",\"locstatecode\":\"WA\",\"loccityid\":3961}]}}\r\n";
            var request = new Uri($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_key}&steamids=76561197960435530");

            mockHandler.SetupGetStringAsync(request, response);

            var client = new HttpClient(mockHandler.Object);

            var steamApi = new SteamApi(client);

            var model = await steamApi.GetPlayerProfileAsync("76561197960435530");

            Assert.AreEqual(model.steamid, "76561197960435530");
            Assert.AreEqual(model.personaname, "Robin");
            Assert.AreEqual(model.avatarfull,
                "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4_full.jpg");
        }

    }
}
