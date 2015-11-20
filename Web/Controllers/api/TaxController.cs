using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Domain;

namespace Web.Controllers.Api
{
    [System.Web.Http.RoutePrefix("api/tax")]
    public class TaxController : ApiController
    {

        [System.Web.Http.Route("template")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Template()
        {
            var vm = new TemplateViewModel();

            vm.HandleRequest();
            return Ok(vm);
        }

        [System.Web.Http.Route("template")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Template(TemplateViewModel vm)
        {
            vm.IsValid = ModelState.IsValid;
            vm.HandleRequest();

            if (vm.IsValid)
            {
                ModelState.Clear();
            }
            else
            {
                foreach (var item in vm.ValidationErrors)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }
            }

            return Ok(vm);
        }

    }

    public class TemplateViewModel
    {
        private const string TEMPLATE_NAME = "DonorTax";

        public TemplateViewModel()
        {
            EventCommand = "Get";
        }

        public string EventCommand { get; set; }
        public Template Template { get; set; }
        public bool IsValid { get; set; }
        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public void HandleRequest()
        {

            switch (EventCommand.ToLower())
            {
                case "get":
                    GetTemplate();
                    break;
                case "save":
                    Save();
                    break;
            }

        }

        private void Save()
        {
            var mgr = new TemplateManager();
            mgr.Update(Template);
        }

        private void GetTemplate()
        {
            var mgr = new TemplateManager();
            Template = mgr.Get(TEMPLATE_NAME);
        }

    }

    public class TemplateManager
    {


        public TemplateManager()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

        public Template Get(string templateName)
        {
            using (var db = new AppContext())
            {
                return db.Templates.FirstOrDefault(t => t.Name == templateName);
            }
        }

        public bool Update(Template template)
        {
            bool ret = false;

            ret = Validate(template);

            if (ret)
            {
                // TODO: Create UPDATE code here
                using (var db = new AppContext())
                {
                    db.Templates.AddOrUpdate(template);
                    db.SaveChanges();
                }
            }

            return ret;
        }

        private bool Validate(Template template)
        {
            ValidationErrors.Clear();

            if (!string.IsNullOrEmpty(template.HeaderText))
            {
                if (template.HeaderText.ToLower() ==
                    template.HeaderText)
                {
                    ValidationErrors.Add(new
                      KeyValuePair<string, string>("Header Text",
                      "Header must not be all lower case."));
                }
            }

            return (ValidationErrors.Count == 0);
        }
    }
}
