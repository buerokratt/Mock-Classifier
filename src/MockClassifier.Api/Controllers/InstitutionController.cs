using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Models;

namespace MockClassifier.Api.Controllers
{
    [Route("input-from-dmr/institution")]
    [ApiController]
    public class InstitutionController : ControllerBase
    {
        [HttpPost]
        public AcceptedResult Post([FromBody] MessagesInput messages)
        {
            return Accepted();
        }
    }
}
