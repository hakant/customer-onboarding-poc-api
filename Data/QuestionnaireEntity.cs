using System.Collections.Generic;

namespace Origin08.CustomerOnboarding.Data
{
    public class QuestionnaireEntity
    {
        public int Id { get; set; }
        public string Questionnaire { get; set; }
    }

    public class Question
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Code { get; set; }
        public string Text { get; set; }
    }
}