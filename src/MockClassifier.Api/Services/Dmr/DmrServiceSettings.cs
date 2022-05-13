namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A settings object for <see cref="DmrService"/>
    /// </summary>
    public class DmrServiceSettings
    {
        private const string DefaultHttpClientName = "DmrApi";
        private const int DefaultHttpRequestTimeoutMs = 30_000;
        private const int DefaultDmrRequestProcessIntervalMs = 5_000;

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

        /// <summary>
        /// The interval in milliseconds between DMR requests processing
        /// </summary>
        public int DmrRequestProcessIntervalMs { get; set; } = DefaultDmrRequestProcessIntervalMs;
    }
}
