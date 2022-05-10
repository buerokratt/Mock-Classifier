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
            return Accepted();
        }
    }
}
