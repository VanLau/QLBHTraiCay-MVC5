using QLBHTraiCay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace QLBHTraiCay.Controllers
{
    public class KiemTraDuLieuController : Controller
    {
        QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();
        // GET: KiemTraDuLieu
        public JsonResult TrungMaSoHangHoa(string maHang, int? id)
        {
            int kq = 0;
            if(id == null)
            {
                kq = db.HangHoas.Count(p => p.MaHang == maHang);
            }
            else
            {
                kq = db.HangHoas.Count(p=>p.ID != id && p.MaHang == maHang);
            }

            if (kq > 0)
            {
                return Json($"Mã số =[{maHang}] bị trùng", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}