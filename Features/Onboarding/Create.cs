using System;
using System.Collections.Generic;
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
    public class Create
    {
        public record Command(string IntakeId) : IRequest<string>;
        
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
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

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var intake = await _context.Intakes
                    .Include(i => i.Answers)
                    .FirstOrDefaultAsync(
                    i => i.IntakeId == command.IntakeId,
                    cancellationToken: cancellationToken
                );

                if (intake is null)
                {
                    // Will probably throw exception later on and handle in a middleware to return proper HTTP codes
                    return null;
                }

                var twoPersons = intake.Answers
                    .Find(a => a.QuestionId == "1")
                    ?.AnswerCode == "two-person";

                var existingOnboarding = await _context.OnboardingWorkflows.FirstOrDefaultAsync(
                    o => o.IntakeId == command.IntakeId,
                    cancellationToken: cancellationToken);

                if (existingOnboarding is not null)
                {
                    return existingOnboarding.OnboardingId;
                }

                var onboarding = new OnboardingWorkflowEntity
                {
                    OnboardingId = Guid.NewGuid().ToString(),
                    IntakeId = command.IntakeId,
                    CreatedDate = DateTime.UtcNow
                };

                var idWorkflows = new List<IdCheckWorkflowEntity>
                {
                    new IdCheckWorkflowEntity
                    {
                        IdCheckWorkflowId = Guid.NewGuid().ToString(),
                        Status = IdCheckStatus.Initial
                    }
                };
                
                if (twoPersons)
                {
                    idWorkflows.Add(new IdCheckWorkflowEntity
                    {
                        IdCheckWorkflowId = Guid.NewGuid().ToString(),
                        Status = IdCheckStatus.Initial
                    });
                }

                onboarding.IdCheckWorkflows = idWorkflows;
                await _context.AddAsync(onboarding, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return onboarding.OnboardingId;
            }
        }
    }
}