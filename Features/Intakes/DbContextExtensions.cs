using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Intakes
{
    public static class DbContextExtensions
    {
        public static async Task<List<Question>> FetchQuestions(
            this CustomerOnboardingContext context,
            CancellationToken cancellationToken)
        {
            var questionnaire = await context.Questionnaires.FirstAsync(cancellationToken);
            var questions = JsonSerializer.Deserialize<List<Question>>(
                questionnaire.Questionnaire
            );

            return questions;
        }
    }
}