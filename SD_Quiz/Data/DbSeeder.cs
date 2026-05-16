using SD_Quiz.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SD_Quiz.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // 1. OTOMATİK ADMİN HESABI OLUŞTURMA
            if (!context.Users.Any(u => u.IsAdmin))
            {
                // Şifreyi AccountController'daki gibi kriptoluyoruz
                string defaultPassword = "admin";
                string hashedPassword = "";

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(defaultPassword));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                    hashedPassword = builder.ToString();
                }

                var adminUser = new User
                {
                    Username = "admin",
                    Password = hashedPassword,
                    IsAdmin = true,
                    Bio = "Sistem Kurucusu ve Yöneticisi",
                    AvatarUrl = "https://cdn-icons-png.flaticon.com/512/9322/9322127.png" // Özel admin rozeti/avatarı
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // 2. VARSAYILAN KATEGORİ VE SORULARI OLUŞTURMA
            if (!context.Categories.Any())
            {
                // Yeni quizler varsayılan olarak IsApproved = false olduğu için, 
                // seeder'ın eklediklerini sistemin kendisi ekliyor kabul edip true yapıyoruz.
                var category1 = new Category { Name = "Araba Modelleri", IsApproved = true };
                var category2 = new Category { Name = "Marka Logoları", IsApproved = true };

                context.Categories.AddRange(category1, category2);
                context.SaveChanges();

                context.Questions.AddRange(
                    new Question
                    {
                        CategoryId = category1.Id,
                        ImageUrl = "https://images.pexels.com/photos/1149831/pexels-photo-1149831.jpeg?auto=compress&cs=tinysrgb&w=600",
                        OptionA = "BMW",
                        OptionB = "Mercedes",
                        OptionC = "Audi",
                        OptionD = "Ford",
                        CorrectAnswer = "C"
                    },
                    new Question
                    {
                        CategoryId = category1.Id,
                        ImageUrl = "https://images.pexels.com/photos/170811/pexels-photo-170811.jpeg?auto=compress&cs=tinysrgb&w=600",
                        OptionA = "Audi",
                        OptionB = "BMW",
                        OptionC = "Toyota",
                        OptionD = "Honda",
                        CorrectAnswer = "B"
                    }
                );

                context.Questions.AddRange(
                    new Question
                    {
                        CategoryId = category2.Id,
                        ImageUrl = "https://images.pexels.com/photos/267394/pexels-photo-267394.jpeg?auto=compress&cs=tinysrgb&w=600",
                        OptionA = "Samsung",
                        OptionB = "Nokia",
                        OptionC = "Apple",
                        OptionD = "Huawei",
                        CorrectAnswer = "C"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}