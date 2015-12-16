using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using EntityFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;
using Web.Models;
using Web.Utilities;

namespace Web.Controllers.api
{
    public class ConstituentApiController : ApiBaseController
    {
        [HttpPost]
        [Route("api/constituents")]
        public IHttpActionResult Get(SearchViewModel vm)
        {
            var page = vm.Page.GetValueOrDefault(0);
            var pageSize = vm.PageSize.GetValueOrDefault(10);
            var skipRows = (page - 1) * pageSize;

            var pred = PredicateBuilder.True<Constituent>();
            if (vm.IsUpdated != null) pred = pred.And(p => p.IsUpdated.HasValue == (vm.IsUpdated ?? null));
            if (!string.IsNullOrWhiteSpace(vm.Name)) pred = pred.And(p => p.Name.Contains(vm.Name));
            if (!string.IsNullOrWhiteSpace(vm.FinderNumber)) pred = pred.And(p => p.FinderNumber.Contains(vm.FinderNumber));
            if (!string.IsNullOrWhiteSpace(vm.LookupId)) pred = pred.And(p => p.LookupId.Contains(vm.LookupId));

            var list = db.Constituents.AsQueryable()
                .Where(pred).OrderBy(x => x.Id).Skip(skipRows).Take(pageSize).ProjectTo<ConstituentViewModel>().ToList();

            var totalCount = db.Constituents.Count();
            var filterCount = db.Constituents.Where(pred).Count();
            var TotalPages = (int)Math.Ceiling((decimal)filterCount / pageSize);

            vm.TotalCount = totalCount;
            vm.FilteredCount = filterCount;
            vm.TotalPages = TotalPages;
            vm.Items = list;

            return Ok(vm);
        }

        [Route("api/constituent/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var vm = db.Constituents.Find(id);
            return Ok(vm);
        }

        [HttpPost]
        [Route("api/constituent")]
        public IHttpActionResult Post(ConstituentViewModel vm)
        {
            var c = Mapper.Map<ConstituentViewModel, Constituent>(vm);
            db.Constituents.AddOrUpdate(c);
            db.SaveChanges();
            return Ok(vm);
        }

        [HttpPost]
        [Route("api/deletetaxitems/")]
        public IHttpActionResult DeleteTaxItems(List<TaxItem> list)
        {
            foreach (var taxItem in list)
            {
                db.TaxItems.Attach(taxItem);
            }
            db.TaxItems.RemoveRange(list);
            db.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("api/constituent/{constituentId:int}/taxes")]
        public IHttpActionResult GetTaxItems(int constituentId)
        {
            var list = db.TaxItems.Where(x => x.ConstituentId == constituentId).ToList();
            return Ok(list);
        }

        [HttpPost]
        [Route("api/constituent/{constituentId:int}/taxes")]
        public IHttpActionResult UpdateTaxItems(int constituentId, List<TaxItem> vm)
        {
            var list = vm;
            EFBatchOperation.For(db, db.TaxItems).UpdateAll(list, x => x.ColumnsToUpdate(c => c.Amount, c => c.DonationDate, c => c.TaxYear));

            return Ok();

        }
    }
}