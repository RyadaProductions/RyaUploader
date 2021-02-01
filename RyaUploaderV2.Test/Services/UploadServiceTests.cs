using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RyaUploaderV2.Facade;
using RyaUploaderV2.Services;

namespace RyaUploaderV2.Test.Services
{
    [TestFixture]
    public class UploadServiceTests
    {
        /// <summary>
        /// Tests wether or not the Uploader is able to succesfully send a ShareCode
        /// </summary>
        /// <returns></returns>
        [Test]
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
