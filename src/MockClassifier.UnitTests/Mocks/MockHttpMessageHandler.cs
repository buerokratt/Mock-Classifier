using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MockClassifier.UnitTests.Mocks
{
    /// <summary>
    /// Mock implementation for the <see cref="HttpMessageHandler"/> to help with tests
    /// </summary>
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static MockHttpMessageHandler ShouldReturnResponse(HttpResponseMessage response) =>
            new((request, cancellationToken) => Task.FromResult(response));

        public static MockHttpMessageHandler ShouldThrowException(Exception exception) =>
            new((request, cancellationToken) => throw exception);

        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> function;

        private MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> function)
        {
            this.function = function;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return function.Invoke(request, cancellationToken);
        }
    }
}
