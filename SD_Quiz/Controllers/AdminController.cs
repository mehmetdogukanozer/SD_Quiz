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

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "True";
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // Sadece durumu "Pending" (Beklemede) olan istekleri getiriyoruz
            var pendingQuizzes = _context.Categories
                .Include(c => c.Questions)
                .Where(c => c.Status == "Pending")
                .ToList();

            return View(pendingQuizzes);
        }

        // YENİ: Adminin Onay Öncesi Quiz Sorularını Görebileceği Önizleme Aksiyonu
        public IActionResult Preview(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var category = _context.Categories
                .Include(c => c.Questions)
                .FirstOrDefault(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category); // Admin/Preview.cshtml sayfasına modeli yolluyoruz
        }

        // Quizi Onayla
        public IActionResult Approve(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsApproved = true;
                category.Status = "Approved"; // Durumu Onaylandı yap
                _context.SaveChanges();
                TempData["Success"] = $"'{category.Name}' başarıyla onaylandı ve yayına alındı!";
            }
            return RedirectToAction("Index");
        }

        // Quizi Reddet (Silme, Durumunu Değiştir)
        public IActionResult Reject(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                category.IsApproved = false;
                category.Status = "Rejected"; // Durumu Reddedildi yap (Kullanıcı görsün diye silmiyoruz)
                _context.SaveChanges();
                TempData["Warning"] = $"'{category.Name}' isimli quiz yayın isteği reddedildi.";
            }
            return RedirectToAction("Index");
        }
    }
}