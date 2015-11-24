using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AutoMapper;
using CsvHelper;
using Domain;
using Microsoft.Ajax.Utilities;
using Web.Infrastructure.Mapping;
using Web.Models;
using AutoMapper.QueryableExtensions;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Web.Utilities;

namespace Web.Controllers.Api
{
    [System.Web.Http.RoutePrefix("api/tax")]
    public class TaxController : ApiBaseController
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


        [System.Web.Http.HttpPost]
        [AsyncTimeout(60000)]
        [System.Web.Http.Route("PostTaxData")]
        public async Task<HttpResponseMessage> PostTaxData()
        {

            var request = HttpContext.Current.Request;

            DatabaseStatusViewModel status;
            if (request.Files.Count == 0)
            {
                status = new DatabaseStatusViewModel()
                {
                    Message = "No files to process"
                };
                return Request.CreateResponse(HttpStatusCode.BadRequest, status);
            }

            var postedFile = request.Files[0];

            var filename = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("\\") + 1);
            var filePath = HttpContext.Current.Server.MapPath(@"~\app_data\" + filename);
            postedFile.SaveAs(filePath);

            status = await LoadDataFile(filePath);

            status.FullFileName = filePath;
            status.FileName = postedFile.FileName;
            status.FileSize = (postedFile.ContentLength / 1024f) / 1024f;
            File.Delete(filePath);
            status.Message = "Successfully processed file";

            return Request.CreateResponse(status.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, status);
        }

        private async Task<DatabaseStatusViewModel> LoadDataFile(string filePath)
        {
            var startTime = DateTime.Now;
            var status = new DatabaseStatusViewModel()
            {
                Success = false,
                RecordsInFile = 0,
                RecordsLoaded = 0
            };

            if (filePath == null)
            {
                status.Message = "No file uploaded";
                return status;
            }

            if (!File.Exists(filePath))
            {
                status.Message = "File does not exist";
                return status;
            }

            using (var csv = new CsvReader(new StreamReader(filePath)))
            {

                //                var csv = new CsvReader(new StreamReader(filePath));
                csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.RegisterClassMap<CsvMap>();

                var records = csv.GetRecords<CsvTaxRecordViewModel>().ToList();
                var csvConstituents = records.DistinctBy(m => m.LookupId).AsQueryable().ProjectTo<ConstituentViewModel>().ToList();
                var constituents = db.Constituents.ProjectTo<ConstituentViewModel>().ToList();
                var map = new Hashtable();


                var missinglist = csvConstituents.Except(constituents, new ConstituentComparer()).ToList();

                // Add new Constituents
                foreach (var vm in missinglist)
                {
                    var constituent = Mapper.Map<ConstituentViewModel, Constituent>(vm);
                    db.Constituents.Add(constituent);
                    db.SaveChanges();
                    map.Add(vm.ConstituentId.Replace("\"", ""), constituent.Id);
                }

                var existingList = csvConstituents.Except(missinglist, new ConstituentComparer());
                //Update existing Constituents information
                foreach (var vm in existingList)
                {
                    ConstituentViewModel cvm = constituents.FirstOrDefault(x => x.ConstituentId == vm.LookupId);
                    if (!vm.Equals(cvm))
                    {
                        vm.Id = cvm.Id;
                        cvm.CopyPropertiesFrom(vm);
                        map.Add(vm.ConstituentId.Replace("\"", ""), vm.Id);

                        var constituent = Mapper.Map<ConstituentViewModel, Constituent>(cvm);
                        db.Constituents.AddOrUpdate(constituent);
                    }

                }
                db.SaveChanges();

                // Remap constituentId to parent table key
                foreach (var vm in records)
                {
                    vm.ConstituentId = (int)map[vm.LookupId];
                }

                var dt = records.ToDataTable();

                var sbc = new SqlBulkCopy(db.Database.Connection.ConnectionString)
                {
                    //HACK: Magic string to define dest table
                    DestinationTableName = "dbo.TaxItems",
                    BatchSize = 10000,
                    BulkCopyTimeout = 0
                };
                //

                sbc.ColumnMappings.Add("TaxYear", "TaxYear");
                sbc.ColumnMappings.Add("DonationDate", "DonationDate");
                sbc.ColumnMappings.Add("Amount", "Amount");
                sbc.ColumnMappings.Add("ConstituentId", "ConstituentId");

                await sbc.WriteToServerAsync(dt);

                //                csv.Dispose();

            }

            status.Success = true;
            status.TotalTime = DateTime.Now.Subtract(startTime).ToString();
            return status;
        }




    }

    public sealed class CsvMap : CsvClassMap<CsvTaxRecordViewModel>
    {
        public CsvMap()
        {
            Map(m => m.LookupId).Name("LookupID");
            Map(m => m.Name).Name("Name");
            Map(m => m.EmailAddress).Name("EmailAddress");
            Map(m => m.Addressline1).Name("Addressline1", "Address Line 1");
            Map(m => m.Addressline2).Name("Addressline2", "Address Line 2");
            Map(m => m.Addressline3).Name("Addressline3", "Address Line 3");
            Map(m => m.City).Name("City");
            Map(m => m.State).Name("State");
            Map(m => m.TaxYear).ConvertUsing(row => row.GetField("Date").ToDateTime().Year);
            Map(m => m.DonationDate).Name("Date");
            Map(m => m.Amount)
                .Name("Amount")
                .ConvertUsing(row => Convert.ToDecimal(row.GetField("Amount")));

        }
    }

    public class ConstituentComparer : IEqualityComparer<ConstituentViewModel>
    {
        public bool Equals(ConstituentViewModel x, ConstituentViewModel y)
        {
            return Equals(x.ConstituentId, y.ConstituentId);
        }

        public int GetHashCode(ConstituentViewModel obj)
        {
            return obj.ConstituentId.GetHashCode();
        }
    }

    public class ConstituentViewModel : IHaveCustomMappings, IEquatable<ConstituentViewModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConstituentId { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string LookupId { get; set; }

        public void CreateMappings(IConfiguration config)
        {
            config.CreateMap<CsvTaxRecordViewModel, ConstituentViewModel>()
                .ForMember(m => m.ConstituentId, opt => opt.MapFrom(u => u.LookupId))
                .ForMember(m => m.Street, opt => opt.MapFrom(u => u.Addressline1))
                .ForMember(m => m.Street2, opt => opt.MapFrom(u => u.Addressline2))
                .ReverseMap();


            config.CreateMap<Constituent, ConstituentViewModel>()
                .ForMember(m => m.ConstituentId, opt => opt.MapFrom(u => u.ConstituentId))
                .ReverseMap();

        }

        public bool Equals(ConstituentViewModel other)
        {
            if (other == null) return false;

            if (other.ConstituentId != ConstituentId) return false;
            if (other.Name != ConstituentId) return false;
            if (other.Street != Street) return false;
            if (other.Street2 != Street2) return false;
            if (other.City != City) return false;
            if (other.State != State) return false;
            if (other.Zipcode != Zipcode) return false;
            if (other.Email != Email) return false;
            if (other.Phone != Phone) return false;

            return true;
        }
    }

    public class CsvTaxRecordViewModel : CsvClassMap<CsvMap>
    {
        public string LookupId { get; set; }
        public int ConstituentId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string Addressline3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DonationDate { get; set; }
        public decimal Amount { get; set; }
        public int TaxYear { get; set; }
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
