﻿using System.Collections.Generic;
using System.Linq;
using RyaUploaderV2.Models;
using RyaUploaderV2.Services.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RyaUploaderV2.Test.Services
{
    [TestClass]
    public class ShareCodeServiceTests
    {
        /// <summary>
        /// Test method intended to check if the ShareCodeConverter is able to convert a Match into a ShareCode.
        /// </summary>
        [TestMethod]
        public void GetNewestDemoUrls_CanProperlyConvertMatchToShareCode()
        {
            var mockProtobuf = new List<Match>
            {
                new Match
                {
                    MatchId = 3253092634687701224,
                    TvPort = 297960105,
                    ReservationId = 3253095767866343686
                }
            };

            var test = new ShareCodeConverter();
            var matchlist = test.ConvertMatchListToShareCodes(mockProtobuf);

            Assert.AreEqual(true, matchlist.Contains("CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP"));
        }
    }
}
