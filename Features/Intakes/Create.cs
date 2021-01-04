using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public class Create
    {
        public record Command(string IntakeId) : IRequest<IntakeEnvelope>;

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
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

        public class Handler : IRequestHandler<Command, IntakeEnvelope>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<IntakeEnvelope> Handle(Command command, CancellationToken cancellationToken)
            {
                var intake = await _context.Intakes.FirstOrDefaultAsync(
                    i => i.IntakeId == command.IntakeId,
                    cancellationToken: cancellationToken
                );

                var questions = await _context.FetchQuestions(cancellationToken);

                if (intake is not null) return new IntakeEnvelope(intake, questions);

                intake = new IntakeEntity
                {
                    IntakeId = command.IntakeId,
                    CreatedDate = DateTime.UtcNow,
                };
                await _context.AddAsync(intake, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return new IntakeEnvelope(intake, questions);
            }
        }
    }
}