using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLBHTraiCay.Models;

namespace QLBHTraiCay.Controllers
{
    [RoutePrefix("quan-ly/trang-chu")]
    public class AdminController : Controller
    {
        QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();
        // GET: Admin
        [Route]
        [Authorize]
        public ActionResult Index()
        {
            int tsCL = db.ChungLoais.Count();
            int tsL = db.Loais.Count();
            int tsHH = db.HangHoas.Count();
            int tsHD = db.HoaDons.Count();
            ViewBag.TongChungLoai = tsCL;
            ViewBag.TongLoai = tsL;
            ViewBag.TongSoHangHoa = tsHH;
            ViewBag.TongSoHoaDon = tsHD;
            return View();
        }
    }
}