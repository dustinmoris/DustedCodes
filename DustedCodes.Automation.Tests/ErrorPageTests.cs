using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DustedCodes.Automation.Framework;
using NUnit.Framework;

namespace DustedCodes.Automation.Tests
{
    [TestFixture, Category("AutomationTests")]
    public class ErrorPageTests
    {
        [Test]
        public async Task Not_Found_Error_Page_Is_Working()
        {
            using (var httpClient = new HttpClient())
            {
                var nonExistingUrl = $"{AppConfig.RootUrl}this/path/does/not/exist";
                var result = await httpClient.GetAsync(new Uri(nonExistingUrl));
                var content = await result.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
                Assert.IsTrue(content.Contains("Page not found"));
                Assert.IsTrue(content.Contains("Dusted Codes"));
            }
        }

        [Test]
        public async Task Bad_Request_Error_Page_Is_Working()
        {
            using (var httpClient = new HttpClient())
            {
                var badUrl = $"{AppConfig.RootUrl}<script></script>";
                var result = await httpClient.GetAsync(new Uri(badUrl));
                var content = await result.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
                Assert.IsTrue(content.Contains("Bad Request"));
                Assert.IsTrue(content.Contains("Dusted Codes"));
            }
        }

        [Test]
        public async Task Forbidden_Error_Page_Is_Shown_On_NewRelic_Folder()
        {
            using (var httpClient = new HttpClient())
            {
                var badUrl = $"{AppConfig.RootUrl}newrelic";
                var result = await httpClient.GetAsync(new Uri(badUrl));
                var content = await result.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
                Assert.IsTrue(content.Contains("Access Forbidden"));
                Assert.IsTrue(content.Contains("Dusted Codes"));
            }
        }
    }
}