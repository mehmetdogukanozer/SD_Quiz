using System.ComponentModel.DataAnnotations;

namespace SD_Quiz.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        // Bu sorunun hangi kategoriye ait olduğunu belirten Foreign Key
        public int CategoryId { get; set; }

        [Required]
        public string ImageUrl { get; set; } // Resmin dosya yolu (Örn: /images/audi.jpg)

        [Required]
        public string OptionA { get; set; }

        [Required]
        public string OptionB { get; set; }

        [Required]
        public string OptionC { get; set; }

        [Required]
        public string OptionD { get; set; }

        [Required]
        [StringLength(1)] // Sadece 'A', 'B', 'C' veya 'D' tutacağı için 1 karakter
        public string CorrectAnswer { get; set; }

        // Sorunun bağlı olduğu kategoriyi kod tarafında da yakalamak için navigation property
        public Category? Category { get; set; }
    }
}