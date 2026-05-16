using System.ComponentModel.DataAnnotations;

namespace SD_Quiz.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; } // Hangi kullanıcı oynadı?
        public int CategoryId { get; set; } // Hangi kategoriyi oynadı?

        public int PointsEarned { get; set; } // Kaç puan aldı?
        public int? Rating { get; set; } // 1 ile 5 arasında yıldız puanı tutacak (Boş bırakılabilir)

        public DateTime DatePlayed { get; set; } = DateTime.Today; // Saat bilgisi olmadan sadece bugünün tarihi

        // İlişkiler (Hangi skora baksak kullanıcısına ve kategorisine gidebilelim diye)
        public User? User { get; set; }
        public Category? Category { get; set; }
    }
}