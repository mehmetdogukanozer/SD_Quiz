using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SD_Quiz.Data;
using System;
using System.Linq;
using System.Collections.Generic;

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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            string username = HttpContext.Session.GetString("Username") ?? "Yarışmacı";
            ViewBag.Username = username;

            // 1. DİNAMİK UNVAN HESAPLAMA
            int totalPoints = _context.Scores
                .Where(s => s.UserId == userId)
                .Sum(s => (int?)s.PointsEarned) ?? 0;

            string rank = "Çaylak 🥚";
            if (totalPoints >= 100) rank = "Efsane 👑";
            else if (totalPoints >= 70) rank = "Altın 🥇";
            else if (totalPoints >= 40) rank = "Gümüş 🥈";
            else if (totalPoints >= 20) rank = "Bronz 🥉";

            ViewBag.Rank = rank;
            ViewBag.TotalPoints = totalPoints;

            // 2. LİDERLİK TABLOSU
            var today = DateTime.Today;
            ViewBag.LeaderboardToday = _context.Scores
                .Where(s => s.DatePlayed == today)
                .GroupBy(s => s.User.Username)
                .Select(g => new LeaderboardViewModel { Username = g.Key, TotalPoints = g.Sum(s => s.PointsEarned) })
                .OrderByDescending(x => x.TotalPoints).Take(5).ToList();

            var sevenDaysAgo = DateTime.Today.AddDays(-7);
            ViewBag.LeaderboardWeekly = _context.Scores
                .Where(s => s.DatePlayed >= sevenDaysAgo)
                .GroupBy(s => s.User.Username)
                .Select(g => new LeaderboardViewModel { Username = g.Key, TotalPoints = g.Sum(s => s.PointsEarned) })
                .OrderByDescending(x => x.TotalPoints).Take(5).ToList();

            // 3. ÖNE ÇIKAN / FAVORİ KATEGORİ
            ViewBag.FavoriteCategoryId = _context.Scores
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            // 4. QUİZLERİ PUANLARINA GÖRE SIRALAMA VE ONAYLI OLANLARI ÇEKME
            var approvedCategories = _context.Categories
                .Where(c => c.IsApproved == true)
                .Include(c => c.Scores) // Puanları hesaplamak için skorları da çekiyoruz
                .Select(c => new CategoryRatingViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    AvgRating = c.Scores.Any(s => s.Rating != null) ? c.Scores.Where(s => s.Rating != null).Average(s => s.Rating).Value : 0
                })
                .OrderByDescending(x => x.AvgRating) // En yüksek puanlı en üste
                .ToList();

            // Favori Kategori (Aynı Kalıyor)
            ViewBag.FavoriteCategoryId = _context.Scores
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return View(approvedCategories); // Modeli View'e gönderiyoruz
        }

        // PROFIL SAYFASI (GET)
        [HttpGet]
        public IActionResult MyProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.Include(u => u.Scores).ThenInclude(s => s.Category).FirstOrDefault(u => u.Id == userId);
            var myScores = _context.Scores.Where(s => s.UserId == userId).ToList();

            ViewBag.GamesPlayed = myScores.Count;
            ViewBag.AverageScore = myScores.Any() ? Math.Round(myScores.Average(s => s.PointsEarned), 1) : 0;
            ViewBag.HighestScore = myScores.Any() ? myScores.Max(s => s.PointsEarned) : 0;

            return View(user);
        }

        // PROFIL GÜNCELLEME (POST)
        [HttpPost]
        public IActionResult UpdateProfile(string bio, string avatarUrl)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Bio = bio;
                if (!string.IsNullOrEmpty(avatarUrl)) user.AvatarUrl = avatarUrl;
                _context.SaveChanges();
                TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi!";
            }
            return RedirectToAction("MyProfile");
        }
    }

    public class LeaderboardViewModel
    {
        public string Username { get; set; }
        public int TotalPoints { get; set; }
    }

    public class CategoryRatingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double AvgRating { get; set; }
    }
}