namespace KaliteKontrol.Viewmodels
{
    public interface IPageControl
    {
        public string PageName { get; set; }
        public void Loaded();
        public void UnLoaded();
    }
}
