using System.ComponentModel.DataAnnotations;

namespace SD_Quiz.Models
{
    public class User
    {
        [Key] // Veritabanındaki benzersiz ID (Primary Key)
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı boş geçilemez.")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre boş geçilemez.")]
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false; // Kullanıcı admin mi?

        // Models/User.cs dosyasının içine, diğer property'lerin altına ekle:
        public string? Bio { get; set; } = "Henüz bir biyografi girilmemiş.";
        public string? AvatarUrl { get; set; } = "https://cdn-icons-png.flaticon.com/512/847/847969.png"; // Varsayılan avatar

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Bir kullanıcının birden fazla skor kaydı olabilir (İlişki tanımı)
        // Soru işareti ekledik, böylece formda bu alanı aramayacak
        public List<Score>? Scores { get; set; }
    }
}