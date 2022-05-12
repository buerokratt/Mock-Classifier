using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Models;

namespace MockClassifier.Api.Controllers
{
    [Route("input-from-dmr/institution")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        /// <summary>
        /// Processes an array of strings to identify corresponding ministries and issues call backs to DMR for each message.
        /// </summary>
        /// <param name="messages">Property which contains an array of strings representing a user message in each string</param>
        /// <returns>An empty 202/Accepted result</returns>
        [HttpPost]
        public AcceptedResult Post([FromBody] MessagesInput messages)
        {
            var minsitries = new List<string>();

            // TO DO
            // Enumerate each string in messages and check for
            // 1) Does the string match one of the pre-defined phrases? If so, work out the corresponding ministry and put it into minsitries
            // 2) Does the string include one of more of the pre-defined tokens? If so, get the corresponding minsitries and put them into minsitries

            foreach (var minsitry in minsitries)
            {
                // TO DO
                // Invoke the DMR call back service to put a message on DMR for each misnitry
            }

            return Accepted();
        }
    }
}