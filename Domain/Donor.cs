namespace Domain
{
    public class Donor: BaseEntity
    {

        public string FinderNumber { get; set; }
        public int? ConstituentId  { get; set; }
        public string DonorType { get; set; }
        public string AccountType { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SourceCode { get; set; }
         
    }
}