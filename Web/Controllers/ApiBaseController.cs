using System.Web.Http;
using Domain;

namespace Web.Controllers
{
    public class ApiBaseController : ApiController
    {
        public readonly AppContext db;

        public ApiBaseController()
        {
            db = new AppContext();
        }
    }
}