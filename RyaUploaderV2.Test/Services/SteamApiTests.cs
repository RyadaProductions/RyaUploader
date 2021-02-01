using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RyaUploaderV2.Facade;
using RyaUploaderV2.Services.SteamServices;
using RyaUploaderV2.Test.Helpers;

namespace RyaUploaderV2.Test.Services
{
    [TestFixture]
    public class SteamApiTests
    {
        private string _key = "3C97DE56AF018917F183FB805DA6B17B";

        [Test]
        public async Task GetPlayerProfile_ReturnsValidPersonModel()
        {
            var mockHandler = new Mock<IHttpClient>();

            var response = "{\"response\":{\"players\":[{\"steamid\":\"76561197960435530\",\"communityvisibilitystate\":3,\"profilestate\":1,\"personaname\":\"Robin\",\"lastlogoff\":1516019754,\"profileurl\":\"http://steamcommunity.com/id/robinwalker/\",\"avatar\":\"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4.jpg\",\"avatarmedium\":\"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4_medium.jpg\",\"avatarfull\":\"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4_full.jpg\",\"personastate\":0,\"realname\":\"Robin Walker\",\"primaryclanid\":\"103582791429521412\",\"timecreated\":1063407589,\"personastateflags\":0,\"loccountrycode\":\"US\",\"locstatecode\":\"WA\",\"loccityid\":3961}]}}\r\n";
            var request = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_key}&steamids=76561197960435530";

            mockHandler.Setup(mk => mk.GetStringAsync(request)).ReturnsAsync(response);

            var steamApi = new SteamApi(mockHandler.Object);

            var model = await steamApi.GetPlayerProfileAsync("76561197960435530");

            Assert.AreEqual(model.steamid, "76561197960435530");
            Assert.AreEqual(model.personaname, "Robin");
            Assert.AreEqual(model.avatarfull,
                "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/f1/f1dd60a188883caf82d0cbfccfe6aba0af1732d4_full.jpg");
        }

    }
}
