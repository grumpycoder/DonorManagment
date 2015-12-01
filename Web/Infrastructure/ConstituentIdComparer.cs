using System.Collections.Generic;
using Web.Controllers.Api;
using Web.Models;

namespace Web.Infrastructure
{
    public class ConstituentIdComparer : IEqualityComparer<ConstituentViewModel>
    {
        public bool Equals(ConstituentViewModel x, ConstituentViewModel y)
        {
            if (x == null && y == null) return true;
            if (x == null | y == null) return false;

            if (x.LookupId == y.LookupId) return true;

            return false;
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