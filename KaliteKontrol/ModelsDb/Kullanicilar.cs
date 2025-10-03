namespace KaliteKontrol.ModelsDb
{
    public class Kullanicilar
    {
        public int Id { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
        public int? YetkiId { get; set; }
        public Yetki? Yetki { get; set; }
    }
}
