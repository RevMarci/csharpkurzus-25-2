using System.Text.Json;
using quiz.Models;

namespace quiz.Services
{
    public class QuizService
    {
        private const string FilePath = "quizzes.json";

        public List<Quiz> getQuizzesInternal()
        {
            if (!File.Exists(FilePath)) return new List<Quiz>();

            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return new List<Quiz>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Quiz>>(json, options) ?? new List<Quiz>();
        }

        public List<QuizPublic> getQuizzesPublic()
        {
            var allQuizzes = getQuizzesInternal();
            var dtos = new List<QuizPublic>();

            foreach (var q in allQuizzes)
            {
                dtos.Add(new QuizPublic
                {
                    Id = q.Id,
                    Title = q.Title,
                    Questions = q.Questions.Select(question => new QuestionPublic
                    {
                        Text = question.Text,
                        Options = question.Options
                    }).ToList()
                });
            }

            return dtos;
        }

        public void createQuiz(Quiz quiz)
        {
            var quizzes = getQuizzesInternal();
            int newId = quizzes.Count > 0 ? quizzes.Max(q => q.Id) + 1 : 1;
            quiz.Id = newId;

            quizzes.Add(quiz);
            saveQuizzes(quizzes);
        }

        public int evaluateQuiz(int quizId, List<int> userAnswers)
        {
            var quizzes = getQuizzesInternal();
            var quiz = quizzes.FirstOrDefault(q => q.Id == quizId);
            if (quiz == null) return 0;

            int score = 0;
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                if (i < userAnswers.Count && userAnswers[i] == quiz.Questions[i].CorrectOptionIndex)
                {
                    score++;
                }
            }
            return score;
        }

        private void saveQuizzes(List<Quiz> quizzes)
        {
            var json = JsonSerializer.Serialize(quizzes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
