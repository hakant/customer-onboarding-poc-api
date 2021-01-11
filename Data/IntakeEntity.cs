using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Origin08.CustomerOnboarding.Data
{
    public class IntakeEntity
    {
        [Key]
        public string IntakeId { get; set; }
        public string CurrentQuestionId { get; set; }
        public List<AnswerEntity> Answers { get; set; } = new();
        public DateTime CreatedDate { get; set; }
    }
}