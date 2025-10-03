using Microsoft.EntityFrameworkCore;

namespace KaliteKontrol.ModelsDb
{
    public class LocalContext : DbContext
    {
        public LocalContext(DbContextOptions options) : base(options)
        {
        }

        //public KaliteContext()
        //{
        //}

        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<Yetki> Yetki { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kullanicilar>().HasData(
                new Kullanicilar
                {
                    Id = 1,
                    KullaniciAdi = "Admin",
                    YetkiId = 1
                },
                new Kullanicilar
                {
                    Id = 2,
                    KullaniciAdi = "Operatör1",
                    YetkiId = 3
                });

            modelBuilder.Entity<Yetki>().HasData(
                new Yetki
                {
                    Id = 1, 
                    YetkiAdi = "Admin",
                },
                new Yetki
                {
                    Id = 2,
                    YetkiAdi = "Ekip Lideri",
                },
                new Yetki
                {
                    Id = 3,
                    YetkiAdi = "Operatör",
                });

            base.OnModelCreating(modelBuilder);
        }

    }
}
