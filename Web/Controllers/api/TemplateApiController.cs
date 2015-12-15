using Domain;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Http;

namespace Web.Controllers.api
{
    public class TemplateApiController : ApiBaseController
    {
        [Route("api/template/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var vm = db.Templates.Find(id);
            return Ok(vm);
        }

        [Route("api/template/{name}")]
        public IHttpActionResult Get(string name)
        {
            var vm = db.Templates.FirstOrDefault(x => x.Name == name);
            return Ok(vm);
        }

        [HttpPost]
        [Route("api/template")]
        public IHttpActionResult Post(Template vm)
        {
            db.Templates.AddOrUpdate(vm);
            db.SaveChanges();
            return Ok(vm);
        }

        [HttpDelete]
        public void Delete(int id)
        {
            var template = db.Templates.Find(id);
            db.Templates.Remove(template);
            db.SaveChanges();
        }
    }
}