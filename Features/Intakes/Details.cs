using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;
using Origin08.CustomerOnboarding.Features.Shared;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public class Details
    {
        public record Query(string IntakeId) : IRequest<IntakeAndQuestionsEnvelope>;

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.IntakeId)
                    .NotNull()
                    .NotEmpty();
                RuleFor(x => x.IntakeId)
                    .Must(Validations.BeValidGuid)
                    .WithMessage("Intake Id has to be a valid Guid.")
                    ;
            }
        }

        public class Handler : IRequestHandler<Query, IntakeAndQuestionsEnvelope>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<IntakeAndQuestionsEnvelope> Handle(Query query, CancellationToken cancellationToken)
            {
                var intake = await _context.Intakes
                    .Include(i => i.Answers)
                    .FirstOrDefaultAsync(
                        i => i.IntakeId == query.IntakeId,
                        cancellationToken: cancellationToken
                    );

                var questions = await _context.FetchQuestions(cancellationToken);

                return intake is not null ? new IntakeAndQuestionsEnvelope(intake, questions) : null;
            }
        }
    }
}