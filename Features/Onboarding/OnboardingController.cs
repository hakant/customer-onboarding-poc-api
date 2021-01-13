using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Origin08.CustomerOnboarding.Features.Onboarding
{
    [ApiController]
    [Route("onboarding")]
    public class OnboardingController : Controller
    {
        private readonly IMediator _mediator;

        public OnboardingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{onboardingId}")]
        public Task<OnboardingWorkflowEnvelope> Get(string onboardingId, CancellationToken cancellationToken)
        {
            return _mediator.Send(new Details.Query(onboardingId), cancellationToken);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> Create(
            [FromBody] Create.Command command,
            CancellationToken cancellationToken
        )
        {
            var onboardingId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Get), new {onboardingId = onboardingId}, onboardingId);
        }

        [HttpPut]
        [Route("{onboardingId}/{idCheckWorkflowId}")]
        public Task<OnboardingWorkflowEnvelope> UpdateIdCheckStatus(
            string onboardingId,
            string idCheckWorkflowId,
            UpdateIdCheckStatusModel idCheckStatus,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateIdCheckStatus.Command(onboardingId, idCheckWorkflowId, idCheckStatus.Status);
            return _mediator.Send(command, cancellationToken);
        }
    }
}