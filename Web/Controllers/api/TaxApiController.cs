using AutoMapper;
using AutoMapper.QueryableExtensions;
using CsvHelper;
using CsvHelper.Configuration;
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
using System.Web.Mvc;
using Web.Infrastructure;
using Web.Infrastructure.Mapping;
using Web.Models;
using Web.Utilities;

namespace Web.Controllers.Api
{
    [System.Web.Http.RoutePrefix("api/tax")]
    public class TaxApiController : ApiBaseController
    {

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