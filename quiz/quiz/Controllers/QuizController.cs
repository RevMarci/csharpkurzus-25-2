using Microsoft.AspNetCore.Mvc;
using quiz.Models;
using quiz.Services;

namespace quiz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly QuizService _quizService;

        public QuizController(QuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost("create")]
        public IActionResult createQuiz([FromBody] Quiz quiz)
        {
            _quizService.createQuiz(quiz);
            return Ok($"Quiz created! ID: {quiz.Id}");
        }

        [HttpGet("list")]
        public ActionResult<List<QuizPublic>> ListQuizzes()
        {
            return _quizService.getQuizzesPublic();
        }

        [HttpPost("submit")]
        public IActionResult submitQuiz([FromBody] QuizSubmission submission)
        {
            var allQuizzes = _quizService.getQuizzesInternal();
            var quiz = allQuizzes.FirstOrDefault(q => q.Id == submission.QuizId);

            if (quiz == null) return NotFound("Quiz not found.");

            int score = _quizService.evaluateQuiz(submission.QuizId, submission.SelectedAnswerIndices);
            int total = quiz.Questions.Count;

            return Ok(new
            {
                QuizTitle = quiz.Title,
                Score = score,
                TotalQuestions = total
            });
        }
    }
}
