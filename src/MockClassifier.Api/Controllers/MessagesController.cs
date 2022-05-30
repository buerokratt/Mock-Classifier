using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services.Dmr;
using System.Text;
using System.Text.Json;

namespace MockClassifier.Api.Controllers
{
    [Route("/dmr-api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDmrService _dmrService;
        private readonly ITokenService _tokenService;
        private readonly INaturalLanguageService _naturalLanguageService;
        private readonly IEncodingService _encodingService;

        public MessagesController(IDmrService dmrService, ITokenService tokenService, INaturalLanguageService naturalLanguageService, IEncodingService encodingService)
        {
            _dmrService = dmrService;
            _tokenService = tokenService;
            _naturalLanguageService = naturalLanguageService;
            _encodingService = encodingService;
        }

        /// <summary>
        /// Processes a string to identify classifications and issues call backs to Dmr for each classification.
        /// </summary>
        /// <param name="input">Property which contains the base64 encoded payload containing the message to be classified</param>
        /// <returns>An empty 202/Accepted result</returns>
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            // Get base64 encoded body as Payload
            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            var input = await reader.ReadToEndAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(input))
            {
                return BadRequest(ModelState);
            }
            var decodedInput = _encodingService.DecodeBase64(input);
            var payload = JsonSerializer.Deserialize<DmrRequestPayload>(decodedInput);

            // Classify
            List<string> classifications = _naturalLanguageService.Classify(payload.Message).ToList();
            classifications = classifications.Concat(_tokenService.Classify(payload.Message).ToList()).ToList();

            // Send Dmr call back(s)
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
                { Constants.SentByHeaderKey, sendToHeader },
                { Constants.MessageIdHeaderKey, messageIdHeader },
                { Constants.SendToHeaderKey, sentByHeader },
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
