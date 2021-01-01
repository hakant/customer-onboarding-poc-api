using Microsoft.EntityFrameworkCore;

namespace Origin08.CustomerOnboarding.Data
{
    public class CustomerOnboardingContext : DbContext
    {
        public CustomerOnboardingContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<IntakeEntity> Intakes { get; set; }
    }
}