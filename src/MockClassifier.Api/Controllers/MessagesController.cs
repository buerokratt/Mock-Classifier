using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services.Dmr;

namespace MockClassifier.Api.Controllers
{
    [Route("/dmr-api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDmrService _dmrService;
        private readonly ITokenService _tokenService;
        private readonly INaturalLanguageService _naturalLanguageService;

        public MessagesController(IDmrService dmrService, ITokenService tokenService, INaturalLanguageService naturalLanguageService)
        {
            _dmrService = dmrService;
            _tokenService = tokenService;
            _naturalLanguageService = naturalLanguageService;
        }

        /// <summary>
        /// Processes a string to identify classifications and issues call backs to Dmr for each classification.
        /// </summary>
        /// <param name="payload">Property which contains the payload containing the message to be classified</param>
        /// <returns>An empty 202/Accepted result</returns>
        [HttpPost]
        public IActionResult Post([FromBody] DmrRequestPayload payload)
        {
            if (payload == null)
            {
                return BadRequest(ModelState);
            }

            List<string> classifications = _naturalLanguageService.Classify(payload.Message).ToList();
            classifications = classifications.Concat(_tokenService.Classify(payload.Message).ToList()).ToList();

            foreach (var classification in classifications)
            {
                var dmrRequest = GetDmrRequest(payload.Message, classification, Request.Headers);
                _dmrService.RecordRequest(dmrRequest);
            }

            return Accepted();
        }

        /// <summary>
        /// Builds a DmrRequest object from parameters
        /// </summary>
        /// <param name="message">The message to go into the .Payload.Message property</param>
        /// <param name="classification">The classification to go into the .Payload.Classification property</param>
        /// <param name="headers">The header collection from the origional Request. Used to create the .Header object</param>
        /// <returns>A DmrRequest object</returns>
        private static DmrRequest GetDmrRequest(string message, string classification, IHeaderDictionary headers)
        {
            // Setup headers
            _ = headers.TryGetValue(Constants.SentByHeaderKey, out var sentByHeader);
            _ = headers.TryGetValue(Constants.MessageIdHeaderKey, out var messageIdHeader);
            _ = headers.TryGetValue(Constants.SendToHeaderKey, out var sendToHeader);
            _ = headers.TryGetValue(Constants.MessageIdRefHeaderKey, out var messageIdRefHeader);
            var dmrHeaders = new Dictionary<string, string>
            {
                { Constants.SentByHeaderKey, sentByHeader },
                { Constants.MessageIdHeaderKey, messageIdHeader },
                { Constants.SendToHeaderKey, sendToHeader },
                { Constants.MessageIdRefHeaderKey, messageIdRefHeader },
            };

            // Setup payload
            var dmrPayload = new DmrRequestPayload()
            {
                Message = message,
                Classification = classification
            };

            // Setup request
            var dmrRequest = new DmrRequest(dmrHeaders)
            {
                Payload = dmrPayload
            };

            return dmrRequest;
        }
    }
}
