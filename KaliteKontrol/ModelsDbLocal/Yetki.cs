namespace KaliteKontrol.ModelsDbLocal
{
    public class Yetki
    {
        public int Id { get; set; }
        public string? YetkiAdi { get; set; }

        public ICollection<Kullanicilar>? Kullanicilar { get; set; }
    }
}
