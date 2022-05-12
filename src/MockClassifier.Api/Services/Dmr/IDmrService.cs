namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// An interface for a service that sends messages to the DMR API
    /// </summary>
    public interface IDmrService
    {
        /// <summary>
        /// Record the given request to be sent to the DMR API later
        /// </summary>
        /// <param name="request">The request object</param>
        void RecordRequest(DmrRequest request);

        /// <summary>
        /// Begin processing requests
        /// </summary>
        Task ProcessRequestsAsync();
    }
}
