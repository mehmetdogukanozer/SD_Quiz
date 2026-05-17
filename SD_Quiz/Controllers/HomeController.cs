#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SD_Quiz.Data;
using SD_Quiz.Models;
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
                .GroupBy(s => new { s.UserId, s.User.Username })
                .Select(g => new LeaderboardViewModel { UserId = g.Key.UserId, Username = g.Key.Username, TotalPoints = g.Sum(s => s.PointsEarned) })
                .OrderByDescending(x => x.TotalPoints).Take(5).ToList();

            var sevenDaysAgo = DateTime.Today.AddDays(-7);
            ViewBag.LeaderboardWeekly = _context.Scores
                .Where(s => s.DatePlayed >= sevenDaysAgo)
                .GroupBy(s => new { s.UserId, s.User.Username })
                .Select(g => new LeaderboardViewModel { UserId = g.Key.UserId, Username = g.Key.Username, TotalPoints = g.Sum(s => s.PointsEarned) })
                .OrderByDescending(x => x.TotalPoints).Take(5).ToList();

            // 3. ÖNE ÇIKAN / FAVORİ KATEGORİ
            ViewBag.FavoriteCategoryId = _context.Scores
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            // 4. QUİZLERİ PUANLARINA GÖRE SIRALAMA (Tercüme Hatası Veren Yer Tamamen Düzeltildi)
            var approvedCategories = _context.Categories
                .Where(c => c.IsApproved == true)
                .Select(c => new CategoryRatingViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    AvgRating = c.Scores.Where(s => s.Rating != null).Average(s => s.Rating) ?? 0
                })
                .OrderByDescending(x => x.AvgRating) // En yüksek puanlı en üste
                .ToList();

            return View(approvedCategories);
        }

        // KENDİ PROFIL SAYFASI (GET)
        [HttpGet]
        public IActionResult MyProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // Sena'nın orijinal sorunsuz EF Core Include zinciri geri getirildi
            var user = _context.Users.Include(u => u.Scores).ThenInclude(s => s.Category).FirstOrDefault(u => u.Id == userId);
            var myScores = _context.Scores.Where(s => s.UserId == userId).ToList();

            ViewBag.GamesPlayed = myScores.Count;
            ViewBag.AverageScore = myScores.Any() ? Math.Round(myScores.Average(s => s.PointsEarned), 1) : 0;
            ViewBag.HighestScore = myScores.Any() ? myScores.Max(s => s.PointsEarned) : 0;

            ViewBag.MyCreatedQuizzes = _context.Categories
                .Include(c => c.Questions)
                .Where(c => c.UserId == userId)
                .ToList();

            return View(user);
        }

        // DIŞARIDAN BİR KULLANICININ PROFİLİNİ GÖRME
        [HttpGet]
        public IActionResult UserProfile(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            if (currentUserId == id) return RedirectToAction("MyProfile");

            var user = _context.Users.Include(u => u.Scores).ThenInclude(s => s.Category).FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            var userScores = _context.Scores.Where(s => s.UserId == id).ToList();

            ViewBag.GamesPlayed = userScores.Count;
            ViewBag.AverageScore = userScores.Any() ? Math.Round(userScores.Average(s => s.PointsEarned), 1) : 0;
            ViewBag.HighestScore = userScores.Any() ? userScores.Max(s => s.PointsEarned) : 0;

            ViewBag.MyCreatedQuizzes = _context.Categories
                .Include(c => c.Questions)
                .Where(c => c.UserId == id)
                .ToList();

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

    // KRİTİK GERİ KAZANIM: Projenin derlenmesini sağlayan modeller eklendi
    public class LeaderboardViewModel
    {
        public int UserId { get; set; }
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