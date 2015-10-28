using System.Web.Mvc;
using Domain;

namespace Web.Controllers
{
    public class BaseController: Controller
    {
        public readonly AppContext db;

        public BaseController()
        {
            db = new AppContext();

        }

//        public BetterJsonResult<T> BetterJson<T>(T model)
//        {
//            return new BetterJsonResult<T>() { Data = model };
//        }
    }
}