using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SD_Quiz.Data;
using System.Linq;

namespace SD_Quiz.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Giriş yapan kişi admin mi kontrolü
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "True";
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // Onay bekleyen quizleri ve soru sayılarını çekiyoruz
            var pendingQuizzes = _context.Categories
                .Include(c => c.Questions)
                .Where(c => c.IsApproved == false)
                .ToList();

            return View(pendingQuizzes);
        }

        // Quizi Onayla
        public IActionResult Approve(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsApproved = true;
                _context.SaveChanges();
                TempData["Success"] = $"'{category.Name}' başarıyla yayına alındı!";
            }
            return RedirectToAction("Index");
        }

        // Quizi Reddet ve Sil
        public IActionResult Reject(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category); // Sorularıyla beraber silinir
                _context.SaveChanges();
                TempData["Warning"] = "Quiz reddedildi ve silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}