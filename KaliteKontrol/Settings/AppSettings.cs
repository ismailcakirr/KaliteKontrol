namespace KaliteKontrol.Settings
{
    public class AppSettings
    {
        public string ComPort { get; set; } = "";
        public int IstNo { get; set; }
        public string PlcIp { get; set; } = "";
        public int DbNo { get; set; }
        public string KameraIp { get; set; } = "";
        //public string KameraIp2 { get; set; } = "";
        //public string KameraIp3 { get; set; } = "";
        public string RtspUrl { get; set; } = "";
        //public string RtspUrl2 { get; set; } = "";
        //public string RtspUrl3 { get; set; } = "";

        public int FotoLimitGB { get; set; } = 10;
        public string FotoPath { get; set; } = "C:\\Resimler";
    }
}
