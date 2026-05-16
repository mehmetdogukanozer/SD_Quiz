using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SD_Quiz.Data;
using SD_Quiz.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SD_Quiz.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // ================= ŞİFRELEME (HASHING) METODU =================
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString(); // Örn: e10adc3949ba59... gibi karmaşık bir metin üretir
            }
        }

        [HttpGet]
        public IActionResult Register() { return View(); }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid) return View(user);

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                return View(user);
            }

            // KULLANICININ ŞİFRESİNİ VERİTABANINA YAZMADAN ÖNCE KRİPTOLUYORUZ! 🔒
            user.Password = HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() { return View(); }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // GİRİLEN ŞİFREYİ KRİPTOLAYIP VERİTABANINDAKİ KRİPTOLU HALİYLE KARŞILAŞTIRIYORUZ 🔒
            string hashedPassword = HashPassword(password);
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);

                // KULLANICI ADMİN Mİ KONTROLÜ
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Hatalı kullanıcı adı veya şifre!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}