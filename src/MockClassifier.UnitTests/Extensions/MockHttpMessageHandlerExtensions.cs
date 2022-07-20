using Buerokratt.Common.Dmr;
using Buerokratt.Common.Encoder;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace MockClassifier.UnitTests.Extensions
{
    internal static class MockHttpMessageHandlerExtensions
    {
        public static MockHttpMessageHandler SetupWithExpectedMessage(
            this MockHttpMessageHandler handler,
            string expectedMessage = "my test message",
            string classification = "border")
        {
            var payload = new DmrRequestPayload
            {
                Message = expectedMessage,
                Classification = classification,
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var jsonPayloadBase64 = new EncodingService().EncodeBase64(jsonPayload);
            _ = handler
                .Expect("/")
                .WithContent(jsonPayloadBase64)
                .Respond(HttpStatusCode.Accepted);

            return handler;
        }
    }
}
