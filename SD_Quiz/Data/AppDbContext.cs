using Microsoft.EntityFrameworkCore;
using SD_Quiz.Models;

namespace SD_Quiz.Data
{
    // DbContext sınıfından miras alarak bu sınıfı bir veritabanı köprüsü yapıyoruz.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanında tabloya dönüşecek olan sınıflarımızı (DbSet) tanımlıyoruz.
        // Sol taraftaki 'User' C# sınıfımız, sağ taraftaki 'Users' ise SQL'deki tablo adı olacak.
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Score> Scores { get; set; }
    }
}