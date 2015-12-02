using System.Collections.Generic;
using Domain;
using Web.Services;

namespace Web.Models
{
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
}