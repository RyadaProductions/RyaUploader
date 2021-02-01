using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RyaUploader.Core.Facade;
using RyaUploader.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RyaUploader.Test.Services
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
            
            var mockHttp = new Mock<IHttpClient>();
            mockHttp.Setup(mk => mk.PostAsync("https://csgostats.gg/match/upload", It.IsAny<HttpContent>())).ReturnsAsync(new HttpResponseMessage{ StatusCode = HttpStatusCode.OK });

            var uploadShareCodeService = new Uploader(mockHttp.Object);
            Assert.IsTrue(await uploadShareCodeService.UploadShareCodes(listOfCodes));
        }
    }
}
