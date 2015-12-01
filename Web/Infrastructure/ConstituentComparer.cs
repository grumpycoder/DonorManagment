using System.Collections.Generic;
using Web.Controllers.Api;
using Web.Models;

namespace Web.Infrastructure
{
    public class ConstituentComparer : IEqualityComparer<ConstituentViewModel>
    {
        public bool Equals(ConstituentViewModel x, ConstituentViewModel y)
        {
            if (x == null && y == null) return true;
            if (x == null | y == null) return false;


            if (x.LookupId != y.LookupId) return false;
            if (x.Name != y.Name) return false;
            if (x.Street != y.Street) return false;
            if (x.Street2 != y.Street2) return false;
            if (x.City != y.City) return false;
            if (x.State != y.State) return false;
            if (x.Zipcode != y.Zipcode) return false;
            if (x.Email != y.Email) return false;
            if (x.Phone != y.Phone) return false;

            return true;

        }

        public int GetHashCode(ConstituentViewModel obj)
        {
            if (obj.LookupId != null)
            {
                return obj.LookupId.GetHashCode();
            }
            return GetHashCode();
        }
    }
}