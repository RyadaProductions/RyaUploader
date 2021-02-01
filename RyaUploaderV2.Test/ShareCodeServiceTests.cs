using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Google.Protobuf.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RyaUploaderV2.ProtoBufs;
using RyaUploaderV2.Services;

namespace RyaUploaderV2.Test
{
    [TestClass]
    public class ShareCodeServiceTests
    {
        [TestMethod]
        public void GetNewestDemoUrls_CanProperlyConvertMatchToShareCode()
        {

            var mockProtobuf = new CMsgGCCStrike15_v2_MatchList();
            var mockMatch = new CDataGCCStrike15_v2_MatchInfo {Matchid = 3253092634687701224};
            // Fix nulled objects Watchablematchinfo and RoundstatsLegacy
            var mockWatchableDemoInfo = new WatchableMatchInfo {TvPort = 297960105};
            mockMatch.Watchablematchinfo = mockWatchableDemoInfo;
            var mockRoundStatsLegacy = new CMsgGCCStrike15_v2_MatchmakingServerRoundStats {Reservationid = 3253095767866343686};
            mockMatch.RoundstatsLegacy = mockRoundStatsLegacy;
            mockProtobuf.Matches.Add(mockMatch);

            var test = new ShareCodeService();
            var matchlist = test.GetNewestDemoUrls(mockProtobuf);

            Assert.AreEqual(true, matchlist.Contains("CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP"));
        }
    }
}
