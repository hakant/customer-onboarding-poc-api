using System.ComponentModel.DataAnnotations;

namespace Origin08.CustomerOnboarding.Data
{
    public enum IdCheckStatus
    {
        Initial,
        PhotosUploaded,
        IdCheckStarted,
        IdCheckTimedOut,
        IdCheckFailed,
        IdCheckSuccessful
    }
    
    public class IdCheckWorkflowEntity
    {
        [Key]
        public string IdCheckWorkflowId { get; set; }
        public string OnboardingWorkflowId { get; set; }
        public IdCheckStatus Status { get; set; }
    }
}