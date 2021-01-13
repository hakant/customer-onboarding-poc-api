using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;
using Origin08.CustomerOnboarding.Features.Shared;

namespace Origin08.CustomerOnboarding.Features.Onboarding
{
    public class Details
    {
        public record Query(string OnboardingId) : IRequest<OnboardingWorkflowEnvelope>;

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.OnboardingId)
                    .NotNull()
                    .NotEmpty()
                    ;
                RuleFor(x => x.OnboardingId)
                    .Must(Validations.BeValidGuid)
                    .WithMessage("Onboarding Id has to be a valid Guid.")
                    ;
            }
        }

        public class Handler : IRequestHandler<Query, OnboardingWorkflowEnvelope>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<OnboardingWorkflowEnvelope> Handle(Query query, CancellationToken cancellationToken)
            {
                var onboarding = await _context.OnboardingWorkflows
                    .Include(i => i.IdCheckWorkflows)
                    .FirstOrDefaultAsync(
                        i => i.OnboardingId == query.OnboardingId,
                        cancellationToken: cancellationToken
                    );

                return onboarding is not null ? new OnboardingWorkflowEnvelope(onboarding) : null;
            }
        }
    }
}