using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Entity = new Constituent();
            SearchEntity = new Constituent();
            TaxItems = new List<TaxItem>();
            EventCommand = "Search";
            IsDetailsVisible = false;
            SelectedTaxYear = DateTime.Now.Year - 1;
            AcceptTerms = false;
        }

        public Constituent SearchEntity { get; set; }
        public Constituent Entity { get; set; }
        public List<TaxItem> TaxItems { get; set; }
        public int SelectedTaxYear { get; set; }
        [Display(Name = "I have read and accept the policy")]
        public bool AcceptTerms { get; set; }
        public decimal TotalTax { get; set; }

        public string EventCommand { get; set; }
        public bool IsDetailsVisible { get; set; }

        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    Get(SearchEntity.ConstituentId);
                    break;
                case "resetsearch":
                    ResetSearch();
                    break;
            }
        }

        private void Get(string constituentId)
        {
            if (!AcceptTerms)
            {
                //display error message
                return;
            }
            if (string.IsNullOrEmpty(constituentId))
            {
                //display error message
                return;
            }
            
            var mgr = new TaxManager();
            Entity = mgr.Get(SearchEntity);
            TotalTax = Entity.TaxItems.Where(t => t.TaxYear == SelectedTaxYear).Sum(x => x.Amount);
            TaxItems = Entity.TaxItems.Where(t => t.TaxYear == SelectedTaxYear).ToList();
            IsDetailsVisible = true;
        }

        private void ResetSearch()
        {
            SearchEntity = new Constituent();
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