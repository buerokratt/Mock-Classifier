using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services.Dmr;

namespace MockClassifier.Api.Controllers
{
    [Route("/classify")]
    [ApiController]
    public class ClassifyController : ControllerBase
    {
        private readonly IDmrService dmrService;

        public ClassifyController(IDmrService dmrService)
        {
            this.dmrService = dmrService;
        }

        /// <summary>
        /// Processes an array of strings to identify corresponding ministries and issues call backs to DMR for each message.
        /// </summary>
        /// <param name="request">The request model for the classify endpoint</param>
        /// <returns>An empty 202/Accepted result</returns>
        [HttpPost]
        public AcceptedResult Post([FromBody] ClassifyRequest request)
        {
            var ministries = new List<string>();

            // TO DO
            // Enumerate each string in messages and check for
            // 1) Does the string match one of the pre-defined phrases? If so, work out the corresponding ministry and put it into minsitries
            // 2) Does the string include one of more of the pre-defined tokens? If so, get the corresponding minsitries and put them into minsitries

            foreach (var ministry in ministries)
            {
                dmrService.RecordRequest(new DmrRequest
                {
                    Payload = new Payload
                    {
                        CallbackUri = request.CallbackUri,
                        Messages = request.Messages,
                        Ministry = ministry
                    }
                });
            }

            return Accepted();
        }
    }
}
