using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Security.Policy;
using System.Web.UI;
using Domain;

namespace Web.Areas.DonorTax.Models
{
    public class TaxViewModel
    {

        public TaxViewModel()
        {
            Entity = new Constituent()
            {
                TaxItems = new List<TaxItem>()
            };
            SearchEntity = new Constituent();
            EventCommand = "List";
        }

        public Constituent SearchEntity { get; set; }
        public Constituent Entity { get; set; }
        public List<TaxItem> TaxItems { get; set; }
        public string EventCommand { get; set; }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    Get(SearchEntity.ConstituentId);
                    break;

            }
        }

        private void Get(string constituentId)
        {
            TaxManager mgr = new TaxManager();
            Entity = mgr.Get(SearchEntity);
        }
    }

    public class TaxManager
    {

        public Constituent Get(Constituent entity)
        {
            using (var db = new AppContext())
            {
                return db.Constituents.Include(t => t.TaxItems).FirstOrDefault(c => c.ConstituentId == entity.ConstituentId);
            }
        }

    }

}