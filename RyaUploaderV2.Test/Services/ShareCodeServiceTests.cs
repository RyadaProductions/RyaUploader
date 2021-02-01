using Microsoft.VisualStudio.TestTools.UnitTesting;
using RyaUploaderV2.ProtoBufs;
using RyaUploaderV2.Services;
using Protobuf = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchList;
using MatchInfo = RyaUploaderV2.ProtoBufs.CDataGCCStrike15_v2_MatchInfo;
using RoundStats = RyaUploaderV2.ProtoBufs.CMsgGCCStrike15_v2_MatchmakingServerRoundStats;

namespace RyaUploaderV2.Test.Services
{
    [TestClass]
    public class ShareCodeServiceTests
    {
        [TestMethod]
        public void GetNewestDemoUrls_CanProperlyConvertLegacyMatchToShareCode()
        {
            var mockProtobuf = new Protobuf
            {
                Matches =
                {
                    new MatchInfo
                    {
                        Matchid = 3253092634687701224,
                        Watchablematchinfo = new WatchableMatchInfo { TvPort = 297960105 },
                        RoundstatsLegacy = new RoundStats { Reservationid = 3253095767866343686 }
                    }
                }
            };

            var test = new ShareCodeService();
            var matchlist = test.ConvertMatchListToShareCodes(mockProtobuf);

            Assert.AreEqual(true, matchlist.Contains("CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP"));
        }

        [TestMethod]
        public void GetNewestDemoUrls_CanProperlyConvertNewMatchToShareCode()
        {
            var mockProtobuf = new Protobuf
            {
                Matches =
                {
                    new MatchInfo
                    {
                        Matchid = 3253092634687701224,
                        Watchablematchinfo = new WatchableMatchInfo { TvPort = 297960105 },
                        Roundstatsall =
                        {
                            new RoundStats { Reservationid = 3253095767866343686 }
                        }
                    }
                }
            };

            var test = new ShareCodeService();
            var matchlist = test.ConvertMatchListToShareCodes(mockProtobuf);

            Assert.AreEqual(true, matchlist.Contains("CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP"));
        }
    }
}
