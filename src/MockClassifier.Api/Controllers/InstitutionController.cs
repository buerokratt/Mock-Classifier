using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services.Dmr;

namespace MockClassifier.Api.Controllers
{
    [Route("input-from-dmr/institution")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly IDmrService _dmrService;
        private readonly ITokenService _tokenService;
        private readonly INaturalLanguageService _naturalLanguageService;

        public InstitutionController(IDmrService dmrService, ITokenService tokenService, INaturalLanguageService naturalLanguageService)
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
        public AcceptedResult Post([FromBody] MessagesInput messages)
        {
            if (messages == null)
            {
        var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Message input empty"
        };
        throw new HttpResponseException(resp);
            }

            foreach (var message in messages.Messages)
            {
                List<string> ministries = _naturalLanguageService.Classify(message).ToList();
                ministries = ministries.Concat(_tokenService.Classify(message).ToList()).ToList();

                foreach (var ministry in ministries)
                {
                    _dmrService.RecordRequest(GetDmrRequest(message, ministry, messages.CallbackUri));
                }
            }
            return Accepted();
        }

        private static DmrRequest GetDmrRequest(string message, string ministry, Uri callbackUri)
        {
            return new DmrRequest
            {
                Payload = new Payload
                {
                    CallbackUri = callbackUri,
                    Messages = new[] { message },
                    Ministry = ministry
                }
            };
        }
    }
}
