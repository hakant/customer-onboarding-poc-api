using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;
using Origin08.CustomerOnboarding.Features.Shared;

namespace Origin08.CustomerOnboarding.Features.Onboarding
{
    public class UpdatePersonalDetails
    {
        public record Command(
            string OnboardingId,
            int PersonIndex,
            string Name,
            string Surname,
            string SocialSecurityNumber) : IRequest<OnboardingWorkflowEnvelope>;

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

                RuleFor(x => x.PersonIndex)
                    .NotNull()
                    .InclusiveBetween(1, 2)
                    ;

                RuleFor(x => x.Name)
                    .NotNull()
                    .NotEmpty()
                    ;

                RuleFor(x => x.Surname)
                    .NotNull()
                    .NotEmpty()
                    ;

                RuleFor(x => x.SocialSecurityNumber)
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
                var onboarding = await _context.OnboardingWorkflows
                    .Include(i => i.PersonalDetails)
                    .FirstOrDefaultAsync(
                        i => i.OnboardingId == command.OnboardingId,
                        cancellationToken: cancellationToken
                    );

                var personalDetailsEntity = onboarding.PersonalDetails
                    .FirstOrDefault(i => i.PersonIndex == command.PersonIndex);

                if (personalDetailsEntity is null)
                    return null;

                personalDetailsEntity.Name = command.Name;
                personalDetailsEntity.Surname = command.Surname;
                personalDetailsEntity.SocialSecurityNumber = command.SocialSecurityNumber;

                await _context.SaveChangesAsync(cancellationToken);

                return new OnboardingWorkflowEnvelope(onboarding);
            }
        }
    }
}