using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
        public Task<IntakeEnvelope> Get([FromQuery] string intakeId, CancellationToken cancellationToken)
        {
            return _mediator.Send(new Details.Query(intakeId), cancellationToken);
        }
        
        [HttpPost]
        public Task<IntakeEnvelope> Create([FromBody]Create.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }
        
        [HttpPut]
        public Task<IntakeEnvelope> Create([FromBody]UpsertAnswer.Command command, CancellationToken cancellationToken)
        {
            return _mediator.Send(command, cancellationToken);
        }
    }
}