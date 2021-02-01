using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public void MatchlistContainsAtLeastOneValue()
        {
            using (var stream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, "Resources", "matches.dat")))
            {
                var protobufList = CMsgGCCStrike15_v2_MatchList.Parser.ParseFrom(stream);
                
                var mockPathService = new Mock<IPathService>(MockBehavior.Strict);
                var mockFileService = new Mock<IFileService>(MockBehavior.Loose);

                mockPathService.Setup(p => p.GetMatchesPath()).Returns("");
                mockFileService.Setup(p => p.ReadMatches("")).Returns(protobufList);

                var test = new ShareCodeService(mockPathService.Object, mockFileService.Object);
                var matchlist = test.GetNewestDemoUrls();

                var result = matchlist.Contains("CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP");
                if (!matchlist.Contains("CSGO-u4Qdr-auYfN-42Qka-dFTwR-tm7SA")) result = false;
                if (!matchlist.Contains("CSGO-BDZZY-cknXp-EqTqH-6uJ2S-rvMpP")) result = false;
                if (!matchlist.Contains("CSGO-YDhs3-JO2u4-HiGrB-Mv587-ipxMM")) result = false;
                if (!matchlist.Contains("CSGO-fVASm-FcDiv-LVkrt-ZPcbM-w77GJ")) result = false;
                if (!matchlist.Contains("CSGO-2zS6H-pjBjj-kLMbF-H5Owx-4L9PL")) result = false;
                if (!matchlist.Contains("CSGO-jUH8H-dJEpz-fZVVs-XzdQB-bzZAB")) result = false;
                if (!matchlist.Contains("CSGO-oeZbS-hVKPJ-fb3N8-ZQNpp-dDWPA")) result = false;

                Assert.IsTrue(result);
            }
        }
    }
}
