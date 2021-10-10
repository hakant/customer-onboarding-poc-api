using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;
using Origin08.CustomerOnboarding.Features.Shared;

namespace Origin08.CustomerOnboarding.Features.Onboarding.PhoneNumber
{
    public class UpsertContactPhoneNumber
    {
        public record Command(string OnboardingId, string ContactPhoneNumber) : IRequest<OnboardingWorkflowEnvelope>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.OnboardingId)
                    .NotNull()
                    .NotEmpty();

                RuleFor(x => x.OnboardingId)
                    .Must(Validations.BeValidGuid)
                    .WithMessage("Onboarding Id has to be a valid Guid.")
                    ;

                RuleFor(x => x.ContactPhoneNumber)
                    .NotNull()
                    .NotEmpty()
                    ;
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
                var onboardingWorkflow = await _context.OnboardingWorkflows
                    .FirstOrDefaultAsync(
                        i => i.OnboardingId == command.OnboardingId,
                        cancellationToken: cancellationToken
                    );

                if (onboardingWorkflow is null) return null;

                onboardingWorkflow.ContactPhoneNumber = command.ContactPhoneNumber;

                await _context.SaveChangesAsync(cancellationToken);

                return new OnboardingWorkflowEnvelope(onboardingWorkflow);
            }
        }
    }
}