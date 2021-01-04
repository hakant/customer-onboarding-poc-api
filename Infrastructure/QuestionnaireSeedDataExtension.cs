using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Infrastructure
{
    public static class WebHostExtensions
    {
        public static async Task<IHost> SeedQuestionnaireData(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<CustomerOnboardingContext>();

            await using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Origin08.CustomerOnboarding.Data.Questionnaires.QuestionnaireV1.json");
            using var reader = new StreamReader(stream!);
            var questionnaire = await reader.ReadToEndAsync();
            await context!.Questionnaires.AddAsync(new QuestionnaireEntity
            {
                Id = 1,
                Questionnaire = questionnaire
            });
            await context.SaveChangesAsync();

            return host;
        }
    }
}