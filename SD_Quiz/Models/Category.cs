using System.ComponentModel.DataAnnotations;

namespace SD_Quiz.Models
{
    public class Category

    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<Question>? Questions { get; set; }
        public List<Score>? Scores { get; set; }

        public bool IsApproved { get; set; } = true;

        // ================= YENİ EKLENEN ALANLAR =================
        public int? UserId { get; set; } // Quizi oluşturan kullanıcının ID'si
        public User? User { get; set; }  // Navigasyon özelliği

        // Durumlar: "Pending" (Beklemede), "Approved" (Onaylandı), "Rejected" (Reddedildi)
        public string Status { get; set; } = "Approved";
    }
}