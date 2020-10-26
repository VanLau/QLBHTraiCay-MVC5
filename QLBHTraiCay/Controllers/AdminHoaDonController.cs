using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBHTraiCay.Models;

namespace QLBHTraiCay.Controllers
{
    [RoutePrefix("Quan-ly/hoa-don")]
    public class AdminHoaDonController : Controller
    {
        private QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();

        #region Danh hóa đơn ajax WebGrid 
        // GET: AdminHoaDon
        [Route("danh-sach-hoa-don")]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            try
            {
                var hoaDons = await db.HoaDons
                            .Include(h => h.HoaDonChiTiets)
                            .ToListAsync();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_IndexPartial", hoaDons);
                }
                return View(hoaDons);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = "không truy cập được dữ liệu <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Chi tiết hóa đơn
        // GET: AdminLoai/Details/5
        [Route("chi-tiet-hoa-don/{id?}")]
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                var hoaDonCTs = await db.HoaDonChiTiets
                                        .Include(p => p.HoaDon)
                                        .Include(p => p.HangHoa)
                                        .Where(p => p.HoaDonID == id)
                                        .ToListAsync();
                if (hoaDonCTs == null)
                {
                    return View("BaoLoi", model: $"Không tìm thấy hóa đơn.");
                }
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_DetailsPartial", hoaDonCTs);
                }
                return View(hoaDonCTs);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = "không truy cập được dữ liệu <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Sửa hoa đơn
        // GET: AdminHoaDon/Edit/5
        [Route("sua-hoa-don/{id?}")]
        [Authorize(Users ="admin@gmail.com")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                HoaDon hoaDon = await db.HoaDons.FindAsync(id);
                if (hoaDon == null) throw new Exception($"Hóa đơn ID={id} không tồn tại.");             
                return View(hoaDon);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminHoaDon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("sua-hoa-don/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@gmail.com")]
        public async Task<ActionResult> Edit(HoaDon hoaDon)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(hoaDon).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                
                return View(hoaDon);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi ghi dữ liệu không thành công.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Search hóa đơn theo Tên 
        [Route]
        [Authorize]
        public async Task<ActionResult> Search(string search)
        {
            try
            {
                HoaDon hoaDon = await db.HoaDons
                                        .FirstOrDefaultAsync(p => p.HoTenKhach.Contains(search));
                if (hoaDon == null) return View("BaoLoi", model: $"Tên khách tìm kiếm:{search} không tồn tại!");

                if (!String.IsNullOrEmpty(search))
                {
                    var hoaDons = await db.HoaDons
                                        .Include(p => p.HoaDonChiTiets)
                                        .Where(p => p.HoTenKhach.Contains(search))
                                        .ToListAsync();
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_IndexPartial", hoaDons);
                    }
                    return View("Index", hoaDons);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                object cauBaoLoi = "không truy cập được dữ liệu <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }

        }
        #endregion

        #region Giải phóng biến
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
