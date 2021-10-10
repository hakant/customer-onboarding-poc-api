namespace Origin08.CustomerOnboarding.Data
{
    public class PersonalDetailsEntity
    {
        public string Id { get; set; }
        public string OnboardingWorkflowId { get; set; }
        public int PersonIndex { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SocialSecurityNumber { get; set; }
    }
}