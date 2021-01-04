using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public class UpsertAnswer
    {
        public record Command(string IntakeId, string QuestionId, string Answer) : IRequest<IntakeEnvelope>;

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

                RuleFor(x => x.QuestionId)
                    .NotNull()
                    .NotEmpty()
                    ;

                RuleFor(x => x.Answer)
                    .NotNull()
                    .NotEmpty()
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
                var intake = await _context.Intakes
                    .Include(i => i.Answers)
                    .FirstOrDefaultAsync(
                    i => i.IntakeId == command.IntakeId,
                    cancellationToken: cancellationToken
                );

                if (intake is null) return null;

                var answers = intake.Answers.Where(
                        a => a.QuestionId != command.QuestionId
                    )
                    .ToList();

                answers.Add(new AnswerEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    QuestionId = command.QuestionId,
                    Answer = command.Answer
                });

                intake.Answers = answers;
                await _context.SaveChangesAsync(cancellationToken);

                return new IntakeEnvelope(intake);
            }
        }
    }
}