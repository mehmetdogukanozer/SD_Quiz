🧠 SD_Quiz Web Uygulaması

SD_Quiz, kullanıcıların farklı kategorilerde (Örn: Marka Logoları, Araba Modelleri vb.) çoktan seçmeli, resimli sorular çözerek puan topladığı ve bu puanlara göre dinamik unvanlar kazandığı ASP.NET Core MVC tabanlı bir yarışma platformudur.

🚀 Proje Özellikleri

Güvenli Kimlik Doğrulama: Session tabanlı, giriş yapmayan kullanıcıları kısıtlayan altyapı.

Günlük Oynama Sınırı: Kullanıcılar bir kategorideki quiz'i günde yalnızca 1 kez çözebilir.

Dinamik Unvan (Rank) Sistemi: Toplanan puanlara göre kullanıcıların rütbesi (Çaylak, Bronz, Gümüş, Altın, Efsane) otomatik güncellenir.

Code-First Veritabanı: Entity Framework Core ile tasarlanmış tam ilişkisel veritabanı mimarisi.

🛠️ Kurulum ve Çalıştırma Rehberi

Projeyi kendi bilgisayarınızda (lokalinizde) sorunsuz bir şekilde ayağa kaldırmak için lütfen aşağıdaki adımları sırasıyla uygulayın.

1. Gereksinimler

Visual Studio 2022 (veya daha güncel bir sürüm)

.NET 10.0 SDK

SQL Server (Örn: SQL Server Express)

2. Projeyi Klonlayın

Projeyi GitHub üzerinden bilgisayarınıza indirin ve Visual Studio ile .sln dosyasını açın.

3. Veritabanı Bağlantısını Ayarlayın (appsettings.json)

Proje, veritabanına bağlanmak için sizin bilgisayarınızdaki SQL Server adını bilmelidir.

Çözüm Gezgini'nden (Solution Explorer) appsettings.json dosyasını açın.

DefaultConnection içerisindeki Server= değerini kendi SQL Server adınızla (SSMS'e girerken kullandığınız Server Name) değiştirin.

Örnek (SQL Express için): Server=.\\SQLEXPRESS;Database=SD_QuizDB;Trusted_Connection=True;TrustServerCertificate=True;

4. Veritabanını İnşa Edin (Migration) ⚠️ ÖNEMLİ

Kodların çalışabilmesi için tabloların veritabanınızda fiziksel olarak oluşturulması gerekmektedir.

Visual Studio üst menüsünden şu yolu izleyin:
Araçlar (Tools) ➡️ NuGet Paket Yöneticisi ➡️ Paket Yöneticisi Konsolu (Package Manager Console)

Alt kısımda açılan konsola aşağıdaki komutu yazın ve Enter'a basın:

Update-Database


Ekranda yeşil/sarı loglar akacak ve işlem başarıyla bittiğinde Done. mesajını göreceksiniz. Bu işlem sayesinde SD_QuizDB adlı veritabanı tablolarıyla birlikte otomatik kurulacaktır.

5. Projeyi Başlatın

Tüm adımları tamamladıktan sonra üstteki yeşil "Start" butonuna basarak (veya F5 tuşu ile) projeyi çalıştırabilir, üyelik oluşturarak test etmeye başlayabilirsiniz! 🎉

Geliştirme Ekibi için Not: Modellerde bir değişiklik yaparsanız Add-Migration <İsim> ve ardından tekrar Update-Database komutlarını çalıştırmayı unutmayın.