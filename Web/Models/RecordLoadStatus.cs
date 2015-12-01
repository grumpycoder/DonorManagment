namespace Web.Models
{
    internal class DatabaseStatusViewModel
    {
        public bool Success { get; set; }
        public int RecordsLoaded { get; set; }
        public string TotalTime { get; set; }
        public int RecordsInFile { get; set; }
        public int ConstituentsUpdated { get; set; }
        public int ConstituentsCreated { get; set; }
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public float FileSize { get; set; }
        public string Message { get; set; }
    }
}