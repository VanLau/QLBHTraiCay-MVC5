using QLBHTraiCay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBHTraiCay.Controllers
{
    public class ChungLoaiController : Controller
    {
        QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();
        #region Danh sách chung loại
        // GET: ChungLoai
        [ChildActionOnly]
        public PartialViewResult _ChungLoaiAjaxPartial()
        {
            try
            {
                List<ChungLoai> chungLoai = db.ChungLoais
                                              .Where(p => p.Loais.Count > 0)
                                              .Include(p => p.Loais)
                                              .ToList();
                ViewBag.ChungLoais = chungLoai;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }
        }
        #endregion
    }
}