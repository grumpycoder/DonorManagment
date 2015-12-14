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
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Domain;
using Microsoft.Ajax.Utilities;
using Web.Infrastructure;
using Web.Infrastructure.Mapping;
using Web.Models;
using Web.Utilities;

namespace Web.Controllers.Api
{
    [System.Web.Http.RoutePrefix("api/tax")]
    public class TaxApiController : ApiBaseController
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
        public IHttpActionResult Constituents(SearchViewModel vm)
        {
            var page = vm.Page.GetValueOrDefault(0);
            var pageSize = vm.PageSize.GetValueOrDefault(10);
            var skipRows = (page - 1) * pageSize;

            var pred = PredicateBuilder.True<Constituent>();
            if(vm.IsUpdated != null) pred = pred.And(p => p.IsUpdated.HasValue == (vm.IsUpdated ?? null));
            if (!string.IsNullOrWhiteSpace(vm.Name)) pred = pred.And(p => p.Name.Contains(vm.Name));
            if(!string.IsNullOrWhiteSpace(vm.FinderNumber)) pred = pred.And(p => p.FinderNumber.Contains(vm.FinderNumber));
            if(!string.IsNullOrWhiteSpace(vm.LookupId)) pred = pred.And(p => p.LookupId.Contains(vm.LookupId));

            var paged = db.Constituents.AsQueryable()
                .Where(pred).OrderBy(x => x.Id).Skip(skipRows).Take(pageSize).ProjectTo<ConstituentViewModel>().ToList();
            var totalCount = db.Constituents.Count();
            var filterCount = db.Constituents.Where(pred).Count();
            var TotalPages = (int) Math.Ceiling((decimal) totalCount/pageSize);

//            var list = new PagedCollection<Constituent>()
//            {
//                Page = page,
//                TotalCount = total,
//                FilteredCount = filterTotal,
//                TotalPages = (int)Math.Ceiling((decimal)total / pageSize),
//                Items = paged
//            };


            vm.TotalCount = totalCount;
            vm.FilteredCount = filterCount;
            vm.TotalPages = TotalPages;
            vm.Items = paged; 

            return Ok(vm);
        }

        [System.Web.Http.HttpPost]
        [AsyncTimeout(60000)]
        [System.Web.Http.Route("PostTaxData")]
        public async Task<HttpResponseMessage> PostTaxData()
        {
            var request = HttpContext.Current.Request;

            if (request.Files.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new DatabaseStatusViewModel() { Message = "No files to process" });
            }

            var postedFile = request.Files[0];

            var filename = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf("\\") + 1);
            var filePath = HttpContext.Current.Server.MapPath(@"~\app_data\" + filename);
            postedFile.SaveAs(filePath);

            var status = await ProcessDataFile(filePath);

            status.FileName = postedFile.FileName;
            status.FileSize = (postedFile.ContentLength / 1024f) / 1024f;
            File.Delete(filePath);
            return Request.CreateResponse(status.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest, status);
        }

        private async Task<DatabaseStatusViewModel> ProcessDataFile(string filePath)
        {
            var startTime = DateTime.Now;

            if (filePath == null || !File.Exists(filePath))
                return new DatabaseStatusViewModel() { Message = "File does not exist or No file was uploaded" };

            var status = new DatabaseStatusViewModel()
            {
                Success = false,
                RecordsInFile = 0,
                RecordsLoaded = 0,
            };

            var config = new CsvConfiguration()
            {
                IsHeaderCaseSensitive = false,
                WillThrowOnMissingField = false,
                IgnoreReadingExceptions = true,
                ThrowOnBadData = false,
                SkipEmptyRecords = true,
            };
            var csv = new CsvReader(new StreamReader(filePath, Encoding.Default, true), config);
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
                if (cvm == null) continue;
                vm.Id = cvm.Id;
                vm.UpdatedBy = "system";
                vm.UpdatedDate = DateTime.Now;
                cvm.CopyPropertiesFrom(vm);
                var constituent = Mapper.Map<ConstituentViewModel, Constituent>(cvm);
                db.Constituents.AddOrUpdate(constituent);
            }
            status.ConstituentsUpdated = db.SaveChanges();

            // Add new Constituents missing from database
            // Bulk copy new Constituent records
            if (newConstituentList.Count > 0)
            {
                foreach (var vm in newConstituentList)
                {
                    vm.CreatedBy = "system";
                    vm.UpdatedBy = "system";
                    vm.CreatedDate = DateTime.Now;
                    vm.UpdatedDate = DateTime.Now;
                }

                var missingTbl = newConstituentList.ToDataTable();
                using (var sbc = new SqlBulkCopy(db.Database.Connection.ConnectionString))
                {
                    sbc.DestinationTableName = db.GetTableName<Constituent>();
                    sbc.BatchSize = 10000;
                    sbc.BulkCopyTimeout = 0;
                    foreach (var col in missingTbl.Columns)
                    {
                        sbc.ColumnMappings.Add(col.ToString(), col.ToString());
                    }
                    try
                    {
                        await sbc.WriteToServerAsync(missingTbl);
                        status.ConstituentsCreated = sbc.RowsCopiedCount();
                    }
                    catch (Exception e)
                    {
                        status.Message = e.Message;
                    }
                }

            }

            // Update constituents because of new bulk copy constituents
            //TODO: Change Created and Updated user to logged in user
            dbConstituents = db.Constituents.ProjectTo<ConstituentViewModel>().ToList();
            // Build dictionary to map database key to csv records LookupId
            var dic = new Dictionary<int, string>();
            dbConstituents.ForEach(x => dic.Add(x.Id, x.LookupId));

            // Update parent key for each tax record
            //csvTaxRecords.ForEach(x => x.ConstituentId = dic.FirstOrDefault(d => d.Value == x.LookupId).Key);
            csvTaxRecords.ForEach((s) =>
            {
                s.ConstituentId = dic.FirstOrDefault(d => d.Value == s.LookupId).Key;
                s.CreatedBy = "system";
                s.UpdatedBy = "system";
                s.CreatedDate = DateTime.Now;
                s.UpdatedDate = DateTime.Now;
            });
            // Bulk insert new tax records
            using (var sbc = new SqlBulkCopy(db.Database.Connection.ConnectionString))
            {
                sbc.DestinationTableName = db.GetTableName<TaxItem>();
                sbc.BatchSize = 10000;
                sbc.BulkCopyTimeout = 0;

                var dt = Mapper.Map<List<CsvTaxRecordViewModel>, List<TaxItem>>(csvTaxRecords).ToDataTable();

                foreach (var col in dt.Columns)
                {
                    sbc.ColumnMappings.Add(col.ToString(), col.ToString());
                }
                try
                {
                    await sbc.WriteToServerAsync(dt);
                    status.RecordsLoaded = sbc.RowsCopiedCount();
                }
                catch (Exception ex)
                {
                    status.Message = ex.Message;
                }
            }

            status.RecordsInFile = csvTaxRecords.Count;
            status.Success = true;
            if (csvTaxRecords.Count != csv.Row - 2)
            {
                status.Message = "Error in file header mappings. Check file headers and try again.";
                status.Success = false;
            }
            else
            {
                status.Success = true;
                status.Message = "Successfully loaded tax records.";
            }

            status.TotalTime = DateTime.Now.Subtract(startTime).ToString(@"hh\:mm\:ss");
            csv.Dispose();
            return status;
        }

    }
}