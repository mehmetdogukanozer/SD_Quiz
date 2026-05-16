# 🧠 SD_Quiz - Etkileşimli ve Topluluk Odaklı Quiz Platformu

SD_Quiz, kullanıcıların farklı kategorilerde yarışabildiği, kendi quizlerini oluşturup toplulukla paylaşabildiği ve aldıkları puanlarla liderlik tablosunda rekabet ettiği tam teşekküllü bir **ASP.NET Core MVC** projesidir. 

Sıradan bir soru-cevap uygulamasının ötesine geçerek; moderasyon sistemi, şifreleme güvenliği, zamanlayıcı mekanizmaları ve dinamik rütbe sistemi ile profesyonel bir platform deneyimi sunar.

---

## ✨ Öne Çıkan Özellikler

### 🎮 Oyun ve Rekabet
* **Zamana Karşı Yarış:** Her soru için 15 saniyelik JS tabanlı geri sayım sayacı.
* **Günde 1 Kez Kuralı:** Kullanıcılar aynı kategorideki quizi günde yalnızca bir kez çözebilir.
* **Liderlik Tablosu (Leaderboard):** Bugünün ve son 7 günün en çok puan toplayan oyuncuları ana sayfada listelenir.
* **Dinamik Rütbe Sistemi:** Kullanıcıların toplam puanlarına göre rütbeleri (Çaylak, Bronz, Gümüş, Altın, Efsane) otomatik güncellenir.

### 🌍 Topluluk ve İçerik Üretimi (UGC)
* **Quiz Oluşturucu:** Kullanıcılar sisteme kendi kategorilerini ve sınırsız sayıda sorularını ekleyebilir.
* **5 Yıldızlı Değerlendirme:** Çözülen quizler topluluk tarafından oylanır. En yüksek puanlı quizler ana sayfada "Topluluğun Favorileri" olarak öne çıkarılır.
* **Kişiselleştirilebilir Profil:** Avatar seçimi, biyografi düzenleme ve detaylı maç geçmişi/istatistik ekranı.

### 🛡️ Güvenlik ve Moderasyon
* **Güçlü Şifreleme:** Kullanıcı parolaları veritabanında `SHA256` algoritması ile tek yönlü kriptolanarak saklanır.
* **Yönetici (Admin) Paneli:** Kullanıcıların ürettiği quizler anında yayınlanmaz. Admin onayından (`IsApproved`) geçtikten sonra ana sayfada listelenir. Adminler uygunsuz içerikleri reddedip silebilir.
* **Otomatik Kurulum:** Sistem ilk kez ayağa kalktığında `DbSeeder` devreye girerek varsayılan yönetici hesabını otomatik oluşturur.

---

## 🛠️ Kullanılan Teknolojiler

* **Backend:** C#, ASP.NET Core MVC (.NET 10)
* **Veritabanı:** Microsoft SQL Server, Entity Framework Core (Code-First Yaklaşımı)
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 5
* **Güvenlik:** `System.Security.Cryptography` (SHA256), Session tabanlı Auth.

---

## 🚀 Kurulum ve Çalıştırma Rehberi

Projeyi kendi bilgisayarınızda (lokalinizde) sorunsuz bir şekilde ayağa kaldırmak için aşağıdaki adımları izleyin:

### 1. Projeyi Klonlayın
```bash
git clone [https://github.com/KULLANICI_ADINIZ/SD_Quiz.git](https://github.com/KULLANICI_ADINIZ/SD_Quiz.git)
````

Visual Studio üzerinden SD_Quiz.sln dosyasını açın.

### 2. Veritabanı Bağlantısını Ayarlayın
appsettings.json dosyasını açın ve DefaultConnection içerisindeki Server= değerini kendi yerel SQL Server adınızla (Örn: .\SQLEXPRESS veya (localdb)\mssqllocaldb) değiştirin.

### 3. Veritabanını İnşa Edin (Migration) ⚠️ ÖNEMLİ
Visual Studio üst menüsünden: Araçlar ➡️ NuGet Paket Yöneticisi ➡️ Paket Yöneticisi Konsolu yolunu izleyin ve açılan terminale şu komutu yazıp Enter'a basın:

PowerShell
```bash
Update-Database
````
Bu işlem, veritabanınızı tablolarıyla birlikte otomatik olarak oluşturacaktır.

### 4. Projeyi Başlatın
F5 tuşuna basarak veya üstteki yeşil Start butonuna tıklayarak projeyi çalıştırın. Sistem ilk açılışta gerekli örnek dataları ve Yönetici (Admin) hesabını veritabanına otomatik ekleyecektir.

### 🔑 Varsayılan Yönetici (Admin) Bilgileri
Sistem özelliklerini ve Admin Panelini test edebilmek için aşağıdaki bilgilerle giriş yapabilirsiniz:

```bash
Kullanıcı Adı: admin

Şifre: admin
````
(Giriş yaptıktan sonra sağ üst menüden 🛡️ Admin Paneli'ne erişebilir, onay bekleyen quizleri yönetebilirsiniz).
