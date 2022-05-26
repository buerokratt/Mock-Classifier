using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Interfaces;
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
        /// Processes an array of strings to identify corresponding ministries and issues call backs to DMR for each message.
        /// </summary>
        /// <param name="messages">Property which contains an array of strings representing a user message in each string</param>
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

        private static DmrRequest GetDmrRequest(string message, string classification, IHeaderDictionary headers)
        {
            // Grab headers
            _ = headers.TryGetValue("X-Sent-By", out var sentByHeader);

            return new DmrRequest
            {
                SentBy = sentByHeader,
                Payload = new DmrRequestPayload
                {
                    Message = message,
                    Classification = classification
                }
            };
        }
    }
}
