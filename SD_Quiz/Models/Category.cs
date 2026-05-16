using System.ComponentModel.DataAnnotations;

namespace SD_Quiz.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // "Marka Logoları" veya "Araba Modelleri"

        // Bir kategoriye ait birden fazla soru olabilir (İlişki tanımı)
        public List<Question>? Questions { get; set; }
        public List<Score>? Scores { get; set; }
        public bool IsApproved { get; set; } = true; // Admin onayından geçti mi? (Eski quizler kaybolmasın diye varsayılan true yapıyoruz, yeniler false olacak)
    }
}