using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;

namespace KaliteKontrol.ModelsDb
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KaliteContext>();

            // Tablolar yoksa oluştur
            context.Database.Migrate();

            if (!context.Yetki.Any())
            {
                context.Yetki.AddRange(
                    new Yetki { YetkiAdi = "Admin" },
                    new Yetki { YetkiAdi = "Ekip Lideri" },
                    new Yetki { YetkiAdi = "Operatör" }
                );
                context.SaveChanges();
            }

            if (!context.Kullanicilar.Any())
            {
                context.Kullanicilar.AddRange(
                    new Kullanicilar { KullaniciAdi = "Admin", Sifre = "1234*-", YetkiId = 1 },
                    new Kullanicilar { KullaniciAdi = "Operatör1", Sifre = "1234", YetkiId = 3 }
                );
                context.SaveChanges();
            }
            if (!context.ISLEMLER.Any())
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "islemler.json");

                if (!File.Exists(jsonPath))
                    throw new FileNotFoundException($"JSON dosyası bulunamadı: {jsonPath}");

                var json = File.ReadAllText(jsonPath);
                var islemler = JsonSerializer.Deserialize<List<ISLEMLER>>(json);

                if (islemler != null && islemler.Count > 0)
                {
                    context.ISLEMLER.AddRange(islemler);
                    context.SaveChanges();
                }
            }

            if (!context.ADIM.Any())
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "adimlar.json");

                if (!File.Exists(jsonPath))
                    throw new FileNotFoundException($"JSON dosyası bulunamadı: {jsonPath}");

                var json = File.ReadAllText(jsonPath);
                var adimlar = JsonSerializer.Deserialize<List<ADIM>>(json);

                if (adimlar != null && adimlar.Count > 0)
                {
                    context.ADIM.AddRange(adimlar);
                    context.SaveChanges();
                }
            }
        }
    }
}
