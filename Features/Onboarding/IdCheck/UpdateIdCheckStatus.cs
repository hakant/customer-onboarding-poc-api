using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;
using Origin08.CustomerOnboarding.Features.Shared;

namespace Origin08.CustomerOnboarding.Features.Onboarding.IdCheck
{
    public class UpdateIdCheckStatus
    {
        public record Command (string OnboardingId, string IdCheckWorkflowId, IdCheckStatus? Status)
            : IRequest<OnboardingWorkflowEnvelope>;
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.OnboardingId)
                    .NotNull()
                    .NotEmpty()
                    ;
                RuleFor(x => x.OnboardingId)
                    .Must(Validations.BeValidGuid)
                    .WithMessage("Onboarding Id has to be a valid Guid.")
                    ;
                RuleFor(x => x.IdCheckWorkflowId)
                    .NotNull()
                    .NotEmpty()
                    ;
                RuleFor(x => x.IdCheckWorkflowId)
                    .Must(Validations.BeValidGuid)
                    .WithMessage("IdCheckWorkflow Id has to be a valid Guid.")
                    ;
                RuleFor(x => x.Status)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, OnboardingWorkflowEnvelope>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<OnboardingWorkflowEnvelope> Handle(Command command, CancellationToken cancellationToken)
            {
                var onboarding = await _context.OnboardingWorkflows
                    .Include(i => i.IdCheckWorkflows)
                    .FirstOrDefaultAsync(
                        i => i.OnboardingId == command.OnboardingId,
                        cancellationToken: cancellationToken
                    );

                var idCheckWorkflow = onboarding.IdCheckWorkflows
                    .FirstOrDefault(i => i.IdCheckWorkflowId == command.IdCheckWorkflowId);

                if (idCheckWorkflow is null)
                {
                    return null;
                }

                if (command.Status != null)
                {
                    idCheckWorkflow.Status = command.Status.Value;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new OnboardingWorkflowEnvelope(onboarding);
            }
        }
    }
}