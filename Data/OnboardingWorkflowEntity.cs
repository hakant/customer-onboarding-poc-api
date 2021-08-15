using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Origin08.CustomerOnboarding.Data
{
    public class OnboardingWorkflowEntity
    {
        [Key]
        public string OnboardingId { get; set; }
        public string IntakeId { get; set; }
        public IntakeEntity Intake { get; set; }
        public IEnumerable<IdCheckWorkflowEntity> IdCheckWorkflows { get; set; }
        public string ContactPhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}