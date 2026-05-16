using Microsoft.AspNetCore.Mvc;
using SD_Quiz.Data;
using System.Linq;

namespace SD_Quiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. GÜVENLİK KONTROLÜ: Session'da UserId var mı?
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Giriş yapmadıysa zorla Login sayfasına yönlendir
                return RedirectToAction("Login", "Account");
            }

            // 2. KULLANICI BİLGİLERİNİ ÇEKME
            string username = HttpContext.Session.GetString("Username") ?? "Yarışmacı";
            ViewBag.Username = username;

            // 3. DİNAMİK UNVAN (RANK) HESAPLAMA
            // Kullanıcının Scores tablosundaki toplam puanını topluyoruz
            int totalPoints = _context.Scores
                .Where(s => s.UserId == userId)
                .Sum(s => (int?)s.PointsEarned) ?? 0; // Eğer hiç skoru yoksa 0 kabul et

            string rank = "Çaylak 🥚";
            if (totalPoints >= 100) rank = "Efsane 👑";
            else if (totalPoints >= 70) rank = "Altın 🥇";
            else if (totalPoints >= 40) rank = "Gümüş 🥈";
            else if (totalPoints >= 20) rank = "Bronz 🥉";

            ViewBag.Rank = rank;
            ViewBag.TotalPoints = totalPoints;

            // 4. KATEGORİLERİ VERİTABANINDAN ÇEKİP SAYFAYA GÖNDERME
            var categories = _context.Categories.ToList();

            return View(categories);
        }
    }
}