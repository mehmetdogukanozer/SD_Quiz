using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SD_Quiz.Data;
using SD_Quiz.Models;
using System;
using System.Linq;

namespace SD_Quiz.Controllers
{
    public class QuizController : Controller
    {
        private readonly AppDbContext _context;

        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Start(int categoryId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var playedToday = _context.Scores.Any(s => s.UserId == userId && s.CategoryId == categoryId && s.DatePlayed.Date == DateTime.Today);
            if (playedToday)
            {
                TempData["Warning"] = "Bu kategorideki bugünkü hakkınızı kullandınız! Yarın tekrar deneyin.";
                return RedirectToAction("Index", "Home");
            }

            var questions = _context.Questions.Where(q => q.CategoryId == categoryId).Select(q => q.Id).OrderBy(x => Guid.NewGuid()).ToList();
            if (!questions.Any())
            {
                TempData["Warning"] = "Bu kategoride henüz soru bulunmuyor.";
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("QuizQuestions", string.Join(",", questions));
            HttpContext.Session.SetInt32("QuizCurrentIndex", 0);
            HttpContext.Session.SetInt32("QuizScore", 0);
            HttpContext.Session.SetInt32("QuizCategoryId", categoryId);

            return RedirectToAction("Play");
        }

        [HttpGet]
        public IActionResult Play()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");

            var questionsStr = HttpContext.Session.GetString("QuizQuestions");
            var currentIndex = HttpContext.Session.GetInt32("QuizCurrentIndex") ?? 0;
            if (string.IsNullOrEmpty(questionsStr)) return RedirectToAction("Index", "Home");

            var questionIds = questionsStr.Split(',').Select(int.Parse).ToList();
            if (currentIndex >= questionIds.Count) return RedirectToAction("Finish");

            var currentQuestionId = questionIds[currentIndex];
            var question = _context.Questions.FirstOrDefault(q => q.Id == currentQuestionId);

            ViewBag.CurrentQuestionNumber = currentIndex + 1;
            ViewBag.TotalQuestions = questionIds.Count;

            return View(question);
        }

        [HttpPost]
        public IActionResult Answer(int questionId, string selectedOption)
        {
            var question = _context.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question != null && question.CorrectAnswer == selectedOption)
            {
                var currentScore = HttpContext.Session.GetInt32("QuizScore") ?? 0;
                HttpContext.Session.SetInt32("QuizScore", currentScore + 10);
            }

            var currentIndex = HttpContext.Session.GetInt32("QuizCurrentIndex") ?? 0;
            HttpContext.Session.SetInt32("QuizCurrentIndex", currentIndex + 1);

            return RedirectToAction("Play");
        }

        [HttpGet]
        public IActionResult Finish()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var categoryId = HttpContext.Session.GetInt32("QuizCategoryId") ?? 0;
            var finalScore = HttpContext.Session.GetInt32("QuizScore") ?? 0;

            var newScore = new Score { UserId = userId.Value, CategoryId = categoryId, PointsEarned = finalScore, DatePlayed = DateTime.Today };
            _context.Scores.Add(newScore);
            _context.SaveChanges(); // Veritabanına kaydettik ve Id'sini aldık

            HttpContext.Session.Remove("QuizQuestions");
            HttpContext.Session.Remove("QuizCurrentIndex");
            HttpContext.Session.Remove("QuizScore");
            HttpContext.Session.Remove("QuizCategoryId");

            // Skoru ve ID'sini sonuç sayfasına yolluyoruz
            return RedirectToAction("Result", new { points = finalScore, scoreId = newScore.Id });
        }

        [HttpGet]
        public IActionResult Result(int points, int scoreId)
        {
            ViewBag.Points = points;
            ViewBag.ScoreId = scoreId;
            return View();
        }

        [HttpPost]
        public IActionResult RateQuiz(int scoreId, int rating)
        {
            var score = _context.Scores.FirstOrDefault(s => s.Id == scoreId);
            if (score != null)
            {
                score.Rating = rating; // Kullanıcının verdiği yıldızı kaydet
                _context.SaveChanges();
                TempData["Success"] = "Değerlendirmeniz için teşekkürler! Puanınız topluluğa katkı sağladı.";
            }
            return RedirectToAction("Index", "Home");
        }

        // --- ADIM 1: QUİZ BAŞLIĞI OLUŞTURMA ---
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName)) return View();

            // Yeni oluşturulan quiz varsayılan olarak ONAYSIZ (IsApproved = false) başlar
            var newCategory = new Category { Name = categoryName, IsApproved = false };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            return RedirectToAction("AddQuestion", new { categoryId = newCategory.Id });
        }

        // --- ADIM 2: SINIRSIZ SORU EKLEME DÖNGÜSÜ ---
        [HttpGet]
        public IActionResult AddQuestion(int categoryId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) return RedirectToAction("Index", "Home");

            ViewBag.CurrentQuestionCount = _context.Questions.Count(q => q.CategoryId == categoryId);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = categoryId;
            return View();
        }

        [HttpPost]
        public IActionResult AddQuestion(Question question, string actionType)
        {
            if (question.CategoryId > 0)
            {
                _context.Questions.Add(question);
                _context.SaveChanges();
            }

            if (actionType == "addMore")
            {
                TempData["Success"] = "Tebrikler! Quiziniz sisteme eklendi. Admin onayından sonra ana sayfada yayınlanacaktır. ⏳";
                return RedirectToAction("AddQuestion", new { categoryId = question.CategoryId });
            }

            TempData["Success"] = "Tebrikler! Quiziniz başarıyla yayınlandı. 🎉";
            return RedirectToAction("Index", "Home");
        }
    }
}