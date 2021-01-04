using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public class Create
    {
        public record Command() : IRequest<string>;

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly CustomerOnboardingContext _context;

            public Handler(CustomerOnboardingContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var intake = new IntakeEntity
                {
                    IntakeId = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.UtcNow,
                };
                await _context.AddAsync(intake, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return intake.IntakeId;
            }
        }
    }
}