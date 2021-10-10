using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Origin08.CustomerOnboarding.Features.Onboarding.Hub;

namespace Origin08.CustomerOnboarding.Features.Onboarding
{
    [ApiController]
    [Route("onboarding")]
    public class OnboardingController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<IdCheckStatusHub, IIdCheckStatusHubClient> _idCheckStatusHub;

        public OnboardingController(
            IMediator mediator,
            IHubContext<IdCheckStatusHub, IIdCheckStatusHubClient> idCheckStatusHub
        )
        {
            _mediator = mediator;
            _idCheckStatusHub = idCheckStatusHub;
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
        [Route("{onboardingId}/contactPhoneNumber/{phoneNumber}")]
        public async Task<OnboardingWorkflowEnvelope> UpsertContactPhoneNumber(
            string onboardingId,
            string phoneNumber,
            CancellationToken cancellationToken
        )
        {
            var command = new UpsertContactPhoneNumber.Command(onboardingId, phoneNumber);
            var result = await _mediator.Send(command, cancellationToken);
            
            return result;
        }
        
        [HttpPut]
        [Route("{onboardingId}/personalDetails/{personIndex}")]
        public async Task<OnboardingWorkflowEnvelope> UpsertPersonalDetails(
            string onboardingId,
            int personIndex,
            UpdatePersonalDetailsModel personalDetails,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdatePersonalDetails.Command(
                onboardingId,
                personIndex,
                personalDetails.Name,
                personalDetails.Surname,
                personalDetails.SocialSecurityNumber);
            
            var result = await _mediator.Send(command, cancellationToken);
            
            return result;
        }

        [HttpPut]
        [Route("{onboardingId}/{idCheckWorkflowId}/{idCheckIndex}")]
        public async Task<OnboardingWorkflowEnvelope> UpdateIdCheckStatus(
            string onboardingId,
            string idCheckWorkflowId,
            int idCheckIndex,
            UpdateIdCheckStatusModel idCheckStatus,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateIdCheckStatus.Command(
                onboardingId,
                idCheckWorkflowId,
                idCheckStatus.Status
            );
            var result = await _mediator.Send(command, cancellationToken);

            await _idCheckStatusHub
                .Clients
                .Group(onboardingId)
                .IdCheckStatusUpdateReceived(
                    new IdCheckStatusUpdate(
                        idCheckWorkflowId,
                        idCheckIndex,
                        idCheckStatus.Status.ToString()
                    )
                );
            return result;
        }
    }
}