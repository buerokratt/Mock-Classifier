namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A configuration object for <see cref="DmrService"/>
    /// </summary>
    public class DmrServiceConfig
    {
        private const string DefaultHttpClientName = "DmrApi";
        private const int DefaultHttpRequestTimeoutMs = 30_000; // 30 seconds

        /// <summary>
        /// The base URI for the DMR REST API
        /// </summary>
        public string DmrApiUri { get; set; }

        /// <summary>
        /// The name of the <see cref="HttpClient"/> for the <see cref="DmrService"/>
        /// </summary>
        public string ClientName { get; set; } = DefaultHttpClientName;

        /// <summary>
        /// The maximum timeout a HTTP request will wait for a response before it drops
        /// </summary>
        public int HttpRequestTimeoutMs { get; set; } = DefaultHttpRequestTimeoutMs;
    }
}
