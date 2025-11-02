namespace KaliteKontrol.ModelsDb
{
    public class ADIM
    {
        public int ID { get; set; }
        public required string TIP_KODU { get; set; }
        public  required string HAT_ADI { get; set; }
        public required string IST_ADI { get; set; }
        public int IST_NO { get; set; }
        public int ADIM_NO { get; set; }
        public int ISLEM_NO { get; set; }
        public required string ISLEM_ADI { get; set; }
        public required string BC_TANIM { get; set; }
        public int SIKICI_NO { get; set; }
        public int PROG_NO { get; set; }
        public int SIKIM_ADEDI { get; set; }
        public int REWORK_SIKICI_NO { get; set; }
        public int REWORK_PROG_NO { get; set; }
        public int SURE { get; set; }
        public string? RESIM_YOLU { get; set; }
        public string? ETIKET { get; set; }

    }
}
