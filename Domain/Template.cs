namespace Domain
{
    public class Template
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] HeaderImage { get; set; }
        public string HeaderText { get; set; }
        public string BodyText { get; set; }
        public string FooterText { get; set; }
        public string FAQText { get; set; }
        public string YesText { get; set; }
        public string NoText { get; set; }
        public string WaitText { get; set; }

    }
}