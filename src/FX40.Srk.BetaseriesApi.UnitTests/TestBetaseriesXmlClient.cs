using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Srk.BetaseriesApi.Clients;
using Srk.BetaseriesApi;
using Moq;

namespace Srk.BetaseriesApi.UnitTests {

    public class TestBetaseriesXmlClient : BetaseriesXmlClient {

        public static string HashString(string value) {
            return BetaseriesXmlClient.HashString(value);
        }

        private static string testApiKey = "unit-test-api-key";

        internal Mock<IHttpRequestWrapper> HttpRequestWrapperMock { get; private set; }

        internal void VerifyHttpRequest() {
            HttpRequestWrapperMock
                .Verify(h => h.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()),
                Times.Once());
        }

        public static string GetRequestString(string action, Dictionary<string, string> dic, bool fromOuter = false) {
            if (fromOuter)
                dic.Add("key", testApiKey);
            var sb = new StringBuilder();
            sb.Append(action);
            sb.Append("?");
            foreach (var item in dic) {
                sb.Append(item.Key);
                sb.Append("=");
                sb.Append(item.Value);
                sb.Append("&");
            }
            return sb.ToString();
        }

        public TestBetaseriesXmlClient(string apiKey, string userAgent, string baseUrl)
            : base(apiKey, userAgent, baseUrl) {

        }

        public TestBetaseriesXmlClient()
            : base(testApiKey, "unit-test-useragent", "http://localhost/") {

            HttpRequestWrapperMock = new Mock<IHttpRequestWrapper>();
            HttpRequestWrapperMock
                .Setup(h => h.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns<string, Dictionary<string, string>>((str, dic) => {
                    return TestBetaseriesXmlClient.GetRequestString(str, dic);
                });

            http = HttpRequestWrapperMock.Object;
        }

        public new string BaseUrl {
            get { return base.BaseUrl; }
        }

        public new string ExecuteQuery(string action, params string[] keyValues) {
            return base.ExecuteQuery(action, keyValues);
        }

        public new string ExecuteQuery(string action, Dictionary<string, string> parameters) {
            return base.ExecuteQuery(action, parameters);
        }

        internal void SetHttpFunc(Func<string, Dictionary<string, string>, string> func) {
            HttpRequestWrapperMock
                .Setup(h => h.ExecuteQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns<string, Dictionary<string, string>>(func);
        }

    }
}
