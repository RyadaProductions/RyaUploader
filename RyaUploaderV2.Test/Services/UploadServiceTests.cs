using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RyaUploaderV2.Services;

namespace RyaUploaderV2.Test.Services
{
    [TestClass]
    public class UploadServiceTests
    {
        /// <summary>
        /// Tests wether or not the Uploader is able to succesfully send a ShareCode
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UploadShareCodes_ReturnsWithoutException()
        {
            var listOfCodes = new List<string> { "CSGO-3V2i2-d2zCP-3bFns-RKunm-WNmkP" };

            var uploadShareCodeService = new Uploader();
            Assert.IsTrue(await uploadShareCodeService.UploadShareCodes(listOfCodes));
        }
    }
}
