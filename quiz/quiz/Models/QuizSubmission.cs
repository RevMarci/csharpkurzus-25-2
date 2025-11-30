namespace quiz.Models
{
    public class QuizSubmission
    {
        public int QuizId { get; set; }
        public List<int> SelectedAnswerIndices { get; set; } = new();
    }
}
