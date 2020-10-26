using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace QLBHTraiCay.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("HangHoaIndex", "HangHoa");
        }
        [Route("gioi-thieu")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Route("lien-he")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ChildActionOnly]
        public int _ThongKeSoNguoiOnline()
        {
            int d = 0;
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Application != null)
            {
                System.Web.HttpContext.Current.Application.Lock();
                d = (int)System.Web.HttpContext.Current.Application["SoNguoiOnline"];
                System.Web.HttpContext.Current.Application.UnLock();
            }
            return d;
        }
    }
}