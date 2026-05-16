using Microsoft.AspNetCore.Mvc;
using SD_Quiz.Data;
using SD_Quiz.Models;
using System.Linq;

namespace SD_Quiz.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        // Veritabanı bağlantımızı Controller içine enjekte ediyoruz (Dependency Injection)
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ================= KULLANICI KAYIT (REGISTER) =================

        // Kayıt Ol sayfasını ekrana getiren action
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt Ol butonuna basıldığında çalışan action
        [HttpPost]
        public IActionResult Register(User user)
        {
            // C# modelindeki [Required] kurallarına uyulmuş mu (kutular dolu mu) kontrol et
            if (!ModelState.IsValid)
            {
                // Eğer kutular boşsa, veritabanına hiç gitme, sayfayı uyarılarla birlikte geri yükle
                return View(user);
            }

            // 1. Aynı kullanıcı adından veritabanında var mı kontrol et
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                return View(user);
            }

            // 2. Her şey yolundaysa kullanıcıyı veritabanına ekle
            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
        

        // ================= KULLANICI GİRİŞİ (LOGIN) =================

        // Giriş Yap sayfasını ekrana getiren action
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Giriş Yap butonuna basıldığında çalışan action
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Veritabanında kullanıcı adı ve şifre eşleşiyor mu bak
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Giriş başarılı! Kullanıcının ID ve Isim bilgisini Session'a atıyoruz
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);

                // Kullanıcıyı ana sayfaya gönder
                return RedirectToAction("Index", "Home");
            }

            // Giriş başarısızsa ekrana hata mesajı gönder
            ViewBag.Error = "Hatalı kullanıcı adı veya şifre!";
            return View();
        }

        // ================= ÇIKIŞ YAP (LOGOUT) =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Tüm oturum verilerini sil
            return RedirectToAction("Login");
        }
    }
}