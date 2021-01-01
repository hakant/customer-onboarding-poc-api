using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public class Details
    {
        public record Query(string IntakeId) : IRequest<IntakeEnvelope>;

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.IntakeId)
                    .NotNull()
                    .NotEmpty();
                RuleFor(x => x.IntakeId)
                    .Must(BeValidGuid)
                    .WithMessage("Intake Id has to be a valid Guid.")
                    ;
            }

            private static bool BeValidGuid(string intakeId)
            {
                return Guid.TryParse(intakeId, out _);
            }
        }

        public class Handler : IRequestHandler<Query, IntakeEnvelope>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<IntakeEnvelope> Handle(Query query, CancellationToken cancellationToken)
            {
                var intake = await _context.Intakes
                    .Include(i => i.Answers)
                    .FirstOrDefaultAsync(
                    i => i.IntakeId == query.IntakeId,
                    cancellationToken: cancellationToken
                );

                return intake is not null ? new IntakeEnvelope(intake) : null;
            }
        }
    }
}