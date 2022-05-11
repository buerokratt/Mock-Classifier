namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// An interface for a service that sends messages to the DMR API
    /// </summary>
    public interface IDmrService
    {
        /// <summary>
        /// Send the given request to the DMR API
        /// </summary>
        /// <param name="request">The request object</param>
        void SendRequest(DmrRequest request);
    }
}
