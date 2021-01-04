using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    [ApiController]
    [Route("intakes")]
    public class IntakesController : Controller
    {
        private readonly IMediator _mediator;

        public IntakesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public Task<IntakeAndQuestionsEnvelope> Get([FromQuery] string intakeId, CancellationToken cancellationToken)
        {
            return _mediator.Send(new Details.Query(intakeId), cancellationToken);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> Create(
            [FromBody] Create.Command command,
            CancellationToken cancellationToken
        )
        {
            var newIntakeId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new {intakeId = newIntakeId}, newIntakeId);
        }

        [HttpPut]
        public Task<IntakeEnvelope> UpsertAnswer(
            [FromBody] UpsertAnswer.Command command,
            CancellationToken cancellationToken
        )
        {
            return _mediator.Send(command, cancellationToken);
        }
    }
}