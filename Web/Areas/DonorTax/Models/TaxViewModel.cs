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
            IsValid = true;

        }

        private const string templateName = "DonorTax";

        public Constituent SearchEntity { get; set; }
        public Constituent Entity { get; set; }
        public List<TaxItem> TaxItems { get; set; }
        public Template Template { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public int SelectedTaxYear { get; set; }
        [Display(Name = "I have read and accept the policy")]
        public bool AcceptTerms { get; set; }
        public decimal TotalTax { get; set; }
        


        public string EventCommand { get; set; }
        public bool IsDetailsVisible { get; set; }
        public bool IsTaxDataAvailable { get; set; }
        public bool IsValid { get; set; }


        public void HandleRequest()
        {
            switch (EventCommand.ToLower())
            {
                case "list":
                case "search":
                    Get(SearchEntity.ConstituentId);
                    GetTemplate();
                    break;
            }
        }

        private void GetTemplate()
        {
                var mgr = new TemplateManager();
                Template = mgr.Get(templateName);
        }

        private void Get(string constituentId)
        {
            var mgr = new TaxManager();

            Entity = mgr.Get(SearchEntity);
            ValidationErrors = mgr.ValidationErrors;

            if (!AcceptTerms) ValidationErrors.Add(new KeyValuePair<string, string>("Accept", "You must accept the policy."));

            if (ValidationErrors.Count > 0) { IsValid = false; }

            if (Entity == null) IsValid = false;

            if(!IsValid) return;

            TotalTax = Entity.TaxItems.Where(t => t.TaxYear == SelectedTaxYear).Sum(x => x.Amount);
            TaxItems = Entity.TaxItems.Where(t => t.TaxYear == SelectedTaxYear).ToList();
            IsTaxDataAvailable = TaxItems.Count > 0;
            IsDetailsVisible = true;
        }


        private void ResetSearch()
        {
            SearchEntity = new Constituent();
        }
    }


    public class TaxManager
    {
        public TaxManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public Constituent Get(Constituent entity)
        {
            using (var db = new AppContext())
            {
                var ret = db.Constituents.Include(t => t.TaxItems).FirstOrDefault(c => c.ConstituentId == entity.ConstituentId && c.Zipcode.Substring(0, 5).Equals(entity.Zipcode));
                Validate(entity);
                if(ret == null) ValidationErrors.Add(new KeyValuePair<string, string>("Not Found", "No tax records found for given supporter."));

                return ret; 
            }
        }

        private bool Validate(Constituent entity)
        {
            ValidationErrors.Clear();

            if (entity.ConstituentId == null) ValidationErrors.Add(new KeyValuePair<string, string>("Supporter Id", "Supporter Id is required."));
            if (entity.Zipcode == null) ValidationErrors.Add(new KeyValuePair<string, string>("Zipcode", "Zipcode is required."));
                
            return ValidationErrors.Count == 0; 
        }
    }

    public class TemplateManager
    {
        public Template Get(string templateName)
        {
            using (var db = new AppContext())
            {
                return db.Templates.FirstOrDefault(t => t.Name == templateName);
            }
        }
    }
}