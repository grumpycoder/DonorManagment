using Domain;
using System.Web.Http;

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