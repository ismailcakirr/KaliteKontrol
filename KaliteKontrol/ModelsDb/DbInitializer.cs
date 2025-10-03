using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // Yetki tablosunu seed et
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
                    new Kullanicilar { KullaniciAdi = "Admin",Sifre="1234*-", YetkiId = 1 },
                    new Kullanicilar { KullaniciAdi = "Operatör1",Sifre="1234", YetkiId = 3 }
                );
                context.SaveChanges();
            }
        }
    }
}
