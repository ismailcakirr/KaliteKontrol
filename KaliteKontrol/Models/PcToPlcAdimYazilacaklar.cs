namespace KaliteKontrol.Models
{
    public class PcToPlcAdimYazilacaklar
    {
        public short AktifAdimNo { get; set; }
        public short AktifIslemNo { get; set; }
        public short SikiciNo { get; set; }
        public short ProgNo { get; set; }
        public short ReworkSikiciNo { get; set; }
        public short ReworkProgNo { get; set; }
        public short Sure { get; set; }

        public PcToPlcAdimYazilacaklar()
        {
            AktifIslemNo = 0;
            AktifAdimNo = 0;
            SikiciNo = 0;
            ProgNo = 0;
            ReworkSikiciNo = 0;
            ReworkProgNo = 0;
            Sure = 0;
        }
    }
}
