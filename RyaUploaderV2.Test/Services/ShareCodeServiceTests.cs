using Microsoft.VisualStudio.TestTools.UnitTesting;
using RyaUploaderV2.ProtoBufs;
using RyaUploaderV2.Services;

namespace RyaUploaderV2.Test.Services
{
    [TestClass]
    public class ShareCodeServiceTests
    {
        [TestMethod]
        public void GetNewestDemoUrls_CanProperlyConvertMatchToShareCode()
        {
            var mockProtobuf = new CMsgGCCStrike15_v2_MatchList
            {
                Matches =
                {
                    new CDataGCCStrike15_v2_MatchInfo
                    {
                        Matchid = 3253092634687701224,
                        Watchablematchinfo = new WatchableMatchInfo { TvPort = 297960105 },
                        RoundstatsLegacy = new CMsgGCCStrike15_v2_MatchmakingServerRoundStats { Reservationid = 3253095767866343686 }
                    }
                }
            };

            var test = new ShareCodeService();
            var matchlist = test.ConvertMatchListToShareCodes(mockProtobuf);

            Assert.AreEqual(true, matchlist.Contains("CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP"));
        }
    }
}
