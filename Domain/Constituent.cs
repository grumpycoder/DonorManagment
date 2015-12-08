using System;
using System.Collections.Generic;
using static System.String;

namespace Domain
{
    public class Constituent : BaseEntity
    {
        public Constituent()
        {
            TaxItems = new List<TaxItem>();
        }
        public string Name { get; set; }
        public string LookupId { get; set; }
        public string FinderNumber { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsUpdated { get; set; }

        public ICollection<TaxItem> TaxItems { get; set; }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //        return false;
        //    if (GetType() != obj.GetType()) return false;

        //    var constituent = (Constituent)obj;
            
        //    if (constituent.LookupId != LookupId) return false;
        //    if (!string.Equals(constituent.Name, Name, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    if (!string.Equals(constituent.Street, Street, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    if (!string.Equals(constituent.Street2, Street2, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    if (!string.Equals(constituent.City, City, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    if (!string.Equals(constituent.State, State, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    if (!string.Equals(constituent.Zipcode, Zipcode, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    if (!string.Equals(constituent.Email, Email, StringComparison.CurrentCultureIgnoreCase)) return false;
        //    return constituent.Phone == Phone;
        //}

        protected bool Equals(Constituent other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(LookupId, other.LookupId, StringComparison.OrdinalIgnoreCase) && string.Equals(FinderNumber, other.FinderNumber, StringComparison.OrdinalIgnoreCase) && string.Equals(Street, other.Street, StringComparison.OrdinalIgnoreCase) && string.Equals(Street2, other.Street2, StringComparison.OrdinalIgnoreCase) && string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase) && string.Equals(State, other.State, StringComparison.OrdinalIgnoreCase) && string.Equals(Zipcode, other.Zipcode, StringComparison.OrdinalIgnoreCase) && string.Equals(Email, other.Email, StringComparison.OrdinalIgnoreCase) && string.Equals(Phone, other.Phone, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Constituent)) return false;
            return Equals((Constituent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode*397) ^ (LookupId != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(LookupId) : 0);
                hashCode = (hashCode*397) ^ (FinderNumber != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(FinderNumber) : 0);
                hashCode = (hashCode*397) ^ (Street != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Street) : 0);
                hashCode = (hashCode*397) ^ (Street2 != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Street2) : 0);
                hashCode = (hashCode*397) ^ (City != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(City) : 0);
                hashCode = (hashCode*397) ^ (State != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(State) : 0);
                hashCode = (hashCode*397) ^ (Zipcode != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Zipcode) : 0);
                hashCode = (hashCode*397) ^ (Email != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Email) : 0);
                hashCode = (hashCode*397) ^ (Phone != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Phone) : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Constituent left, Constituent right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Constituent left, Constituent right)
        {
            return !Equals(left, right);
        }
    
    }
}