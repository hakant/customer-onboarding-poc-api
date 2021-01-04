using System.Collections.Generic;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public record IntakeAndQuestionsEnvelope(IntakeEntity Intake, List<Question> Questions);
}