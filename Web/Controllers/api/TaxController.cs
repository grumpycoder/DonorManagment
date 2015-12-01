using AutoMapper;
using AutoMapper.QueryableExtensions;
using CsvHelper;
using Domain;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Web.Infrastructure;
using Web.Infrastructure.Mapping;
using Web.Models;
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

            status.Success = true;
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

            using (var csv = new CsvReader(new StreamReader(filePath, Encoding.Default, true)))
            {

                csv.Configuration.WillThrowOnMissingField = false;
                csv.Configuration.RegisterClassMap<CsvMap>();

                var csvTaxRecords = csv.GetRecords<CsvTaxRecordViewModel>().ToList();
                var csvConstituents = csvTaxRecords.DistinctBy(m => m.LookupId).AsQueryable().ProjectTo<ConstituentViewModel>().ToList();
                var dbConstituents = db.Constituents.ProjectTo<ConstituentViewModel>().ToList();

                var newConstituentList = csvConstituents.Except(dbConstituents, new ConstituentIdComparer()).ToList();
                var existingConstituentList = csvConstituents.Except(newConstituentList, new ConstituentIdComparer());
                var constituentChangeList = existingConstituentList.Except(dbConstituents, new ConstituentComparer());


                // Update existing constituents that differ from database
                foreach (var vm in constituentChangeList)
                {
                    ConstituentViewModel cvm = dbConstituents.FirstOrDefault(x => x.LookupId == vm.LookupId);
                    vm.Id = cvm.Id;
                    cvm.CopyPropertiesFrom(vm);
                    var constituent = Mapper.Map<ConstituentViewModel, Constituent>(cvm);
                    db.Constituents.AddOrUpdate(constituent);
                }
                status.ConstituentsUpdated = db.SaveChanges();


                // Add new Constituents missing from database
                // Bulk copy new Constituent records
                if (newConstituentList.Count > 0)
                {
                    var missingTbl = newConstituentList.ToDataTable();
                    using (var sbc = new SqlBulkCopy(db.Database.Connection.ConnectionString))
                    {
                        sbc.DestinationTableName = "dbo.Constituents";
                        sbc.BatchSize = 10000;
                        sbc.BulkCopyTimeout = 0;

                        sbc.ColumnMappings.Add("LookupId", "LookupId");
                        sbc.ColumnMappings.Add("Name", "Name");
                        sbc.ColumnMappings.Add("Street", "Street");
                        sbc.ColumnMappings.Add("Street2", "Street2");
                        sbc.ColumnMappings.Add("City", "City");
                        sbc.ColumnMappings.Add("State", "State");
                        sbc.ColumnMappings.Add("Zipcode", "Zipcode");
                        sbc.ColumnMappings.Add("Email", "Email");
                        sbc.ColumnMappings.Add("Phone", "Phone");
                        await sbc.WriteToServerAsync(missingTbl);
                    }
                }
                status.ConstituentsCreated = newConstituentList.Count;

                // Build dictionary to map database key to csv records LookupId
                var dic = new Dictionary<int, string>();
                dbConstituents.ForEach(x => dic.Add(x.Id, x.LookupId));

                // Update parent key for each tax record
                csvTaxRecords.ForEach(x => x.ConstituentId = dic.FirstOrDefault(d => d.Value == x.LookupId).Key);

                // Bulk insert new tax records
                using (var sbc = new SqlBulkCopy(db.Database.Connection.ConnectionString))
                {
                    sbc.DestinationTableName = "dbo.TaxItems";
                    sbc.BatchSize = 10000;
                    sbc.BulkCopyTimeout = 0;

                    sbc.ColumnMappings.Add("TaxYear", "TaxYear");
                    sbc.ColumnMappings.Add("DonationDate", "DonationDate");
                    sbc.ColumnMappings.Add("Amount", "Amount");
                    sbc.ColumnMappings.Add("ConstituentId", "ConstituentId");

                    var dt = csvTaxRecords.ToDataTable();

                    await sbc.WriteToServerAsync(dt);
                }

                status.RecordsInFile = csvTaxRecords.Count;
                CsvTaxRecordViewModel csvTaxRecordViewModel = csvTaxRecords.FirstOrDefault();
                if (csvTaxRecordViewModel != null)
                {
                    var taxYear = csvTaxRecordViewModel.TaxYear;
                    status.RecordsLoaded = db.TaxItems.Count(t => t.TaxYear == taxYear);
                }
            }

            status.Success = true;
            status.TotalTime = DateTime.Now.Subtract(startTime).ToString(@"hh\:mm\:ss");
            return status;
        }

    }
}
