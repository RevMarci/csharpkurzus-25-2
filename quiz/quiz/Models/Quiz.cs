namespace quiz.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<Question> Questions { get; set; } = new();
    }

    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
    }

    public class QuizPublic
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuestionPublic> Questions { get; set; } = new();
    }

    public class QuestionPublic
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }
}
