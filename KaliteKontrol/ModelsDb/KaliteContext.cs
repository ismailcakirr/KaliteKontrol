using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace KaliteKontrol.ModelsDb
{
    public class KaliteContext : DbContext
    {
        public KaliteContext(DbContextOptions<KaliteContext> options) : base(options)
        {
        }

        public KaliteContext()
        {
        }

        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<Yetki> Yetki { get; set; }
        public DbSet<ADIM> ADIM { get; set; }
        public DbSet<ISLEMLER> ISLEMLER { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
    

            modelBuilder.Entity<Kullanicilar>().Property(u => u.KullaniciAdi).HasMaxLength(50);
            modelBuilder.Entity<Kullanicilar>().Property(u => u.KullaniciAdi).IsRequired();
            modelBuilder.Entity<Kullanicilar>().HasIndex(u => u.KullaniciAdi).IsUnique();
            modelBuilder.Entity<Kullanicilar>().Property(u => u.Sifre).IsRequired();   
            modelBuilder.Entity<Kullanicilar>().Property(u => u.Sifre).HasMaxLength(12);
            modelBuilder.Entity<Yetki>().Property(y => y.YetkiAdi).IsRequired();
            modelBuilder.Entity<Yetki>().Property(y => y.YetkiAdi).HasMaxLength(20);
            
            modelBuilder.Entity<ADIM>().Property(a => a.TIP_KODU).IsRequired();
            //modelBuilder.Entity<ADIM>().HasIndex(a => a.TIP_KODU).IsUnique();
            modelBuilder.Entity<ADIM>().Property(a => a.TIP_KODU).HasMaxLength(16);           
            modelBuilder.Entity<ADIM>().Property(a => a.HAT_ADI).HasMaxLength(50);         
            modelBuilder.Entity<ADIM>().Property(a => a.IST_ADI).HasMaxLength(50);         
            modelBuilder.Entity<ADIM>().Property(a => a.ISLEM_ADI).HasMaxLength(150);          
            modelBuilder.Entity<ADIM>().Property(a => a.BC_TANIM).HasMaxLength(150);          
            modelBuilder.Entity<ADIM>().Property(a => a.RESIM_YOLU).HasMaxLength(250);
            modelBuilder.Entity<ADIM>().Property(a => a.ETIKET).HasMaxLength(250);

            modelBuilder.Entity<ISLEMLER>().Property(i => i.ISLEM_ADI).IsRequired();  
            modelBuilder.Entity<ISLEMLER>().Property(i => i.ISLEM_ADI).HasMaxLength(150);       
            modelBuilder.Entity<ISLEMLER>().HasIndex(u => u.ISLEM_NO).IsUnique();


            base.OnModelCreating(modelBuilder);
        }

    }
}
