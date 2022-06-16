using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Interfaces;
using RequestProcessor.AsyncProcessor;
using RequestProcessor.Dmr;
using RequestProcessor.Models;
using RequestProcessor.Services.Encoder;
using System.Text;
using System.Text.Json;

namespace MockClassifier.Api.Controllers
{
    [Route("/dmr-api/messages")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IAsyncProcessorService<DmrRequest> _dmrService;
        private readonly ITokenService _tokenService;
        private readonly INaturalLanguageService _naturalLanguageService;
        private readonly IEncodingService _encodingService;

        public MessagesController(
            IAsyncProcessorService<DmrRequest> dmrService,
            ITokenService tokenService,
            INaturalLanguageService naturalLanguageService,
            IEncodingService encodingService)
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
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post()
        {
            // Get base64 encoded body as Payload
            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            var input = await reader.ReadToEndAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(input))
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine(input);

            var decodedInput = _encodingService.DecodeBase64(input);
            var payload = JsonSerializer.Deserialize<DmrRequestPayload>(decodedInput);

            // Classify
            List<string> classifications = _naturalLanguageService.Classify(payload.Message).ToList();
            classifications.AddRange(_tokenService.Classify(payload.Message).ToList());

            // Send Dmr call back(s)
            foreach (var classification in classifications)
            {
                var dmrRequest = GetDmrRequest(payload.Message, classification, Request.Headers);
                _dmrService.Enqueue(dmrRequest);
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
            _ = headers.TryGetValue(Constants.XSentByHeaderName, out var sentByHeader);
            _ = headers.TryGetValue(Constants.XMessageIdHeaderName, out var messageIdHeader);
            _ = headers.TryGetValue(Constants.XSendToHeaderName, out var sendToHeader);
            _ = headers.TryGetValue(Constants.XModelTypeHeaderName, out var modelTypeHeader);

            // Setup payload
            var dmrPayload = new DmrRequestPayload()
            {
                Message = message,
                Classification = classification
            };

            var dmrHeaders = new HeadersInput
            {
                XSentBy = sendToHeader,
                XMessageId = Guid.NewGuid().ToString(),
                XSendTo = sentByHeader,
                XMessageIdRef = messageIdHeader,
                XModelType = modelTypeHeader
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
