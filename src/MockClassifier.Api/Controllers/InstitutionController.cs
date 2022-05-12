using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services;

namespace MockClassifier.Api.Controllers
{
    [Route("input-from-dmr/institution")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        private readonly INaturalLanguageService naturalLanguageService;
        private readonly ITokenService tokenService;

        public InstitutionController()
        {
            naturalLanguageService = new NaturalLanguageService();
            tokenService = new TokenService();
        }

        /// <summary>
        /// Processes an array of strings to identify corresponding ministries and issues call backs to DMR for each message.
        /// </summary>
        /// <param name="messages">Property which contains an array of strings representing a user message in each string</param>
        /// <returns>An empty 202/Accepted result</returns>
        [HttpPost]
        public AcceptedResult Post([FromBody] MessagesInput messages)
        {
            var ministries = new List<string>();

            foreach( var messageBody in messages.Messages)
            {
                // Enumerate each string in messages and check for
                // 1) Does the string match one of the pre-defined phrases? If so, work out the corresponding ministry and put it into minsitries
                // 2) Does the string include one of more of the pre-defined tokens? If so, get the corresponding minsitries and put them into minsitries
                ministries = naturalLanguageService.Classify(messageBody).ToList();
                ministries = ministries.Concat(tokenService.Classify(messageBody).ToList()).ToList();
            }

            foreach (var ministry in ministries)
            {
                // TO DO
                // Invoke the DMR call back service to put a message on DMR for each misnitry
            }

            return Accepted();
        }
    }
}
