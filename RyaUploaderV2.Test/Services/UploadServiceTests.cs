using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RyaUploaderV2.Services;

namespace RyaUploaderV2.Test.Services
{
    [TestClass]
    public class UploadServiceTests
    {
        [TestMethod]
        public async Task UploadShareCodes_ReturnsWithoutException()
        {
            var listOfCodes = new List<string> { "CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP" };

            var uploadShareCodeService = new UploadService();
            Assert.IsTrue(await uploadShareCodeService.UploadShareCodes(listOfCodes));
        }
    }
}
