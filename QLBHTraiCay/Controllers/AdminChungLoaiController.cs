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
using System.Web.UI;

namespace QLBHTraiCay.Controllers
{
    [RoutePrefix("quan-ly/chung-loai")]
    [Authorize]
    public class AdminChungLoaiController : Controller
    {
        private QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();

        #region Danh sách chủng loại
        // GET: AdminChungLoai
        [Route("danh-sach-chung-loai")]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            try
            {
                var chungLoais = await db.ChungLoais
                                         .ToListAsync();
                return View(chungLoais);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Chi tiết chủng loại
        // GET: AdminChungLoai/Details/5
        [Route("chi-tiet-chung-loai/{id?}")]
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                ChungLoai chungLoai = await db.ChungLoais.FindAsync(id);
                if (chungLoai == null)
                {
                    return View("BaoLoi", model: $"Không tìm thấy loại hàng.");
                }
                return View(chungLoai);
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Không truy cập được dữ liệu. {ex.Message}");
            }
        }
        #endregion

        #region Thêm mới chủng loại
        // GET: AdminChungLoai/Create
        [Route("them-moi-chung-loai")]
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Không truy cập được dữ liệu. {ex.Message}");
            }
        }

        // POST: AdminChungLoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("them-moi-chung-loai")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create( ChungLoai chungLoai)
        {
            try
            {
                int d = db.ChungLoais.Count(p => p.MaCL == chungLoai.MaCL);
                if (d > 0) ModelState.AddModelError("MaCL", $"Mã số {chungLoai.MaCL} bị trùng.");
                if (ModelState.IsValid)
                {
                    db.ChungLoais.Add(chungLoai);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(chungLoai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi ghi dữ liệu không thành công.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Sửa chủng loại
        // GET: AdminChungLoai/Edit/5
        [Route("sua-chung-loai/{id?}")]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                ChungLoai chungLoai = await db.ChungLoais.FindAsync(id);
                if (chungLoai == null) throw new Exception($"Chủng loại ID={id} không tồn tại.");

                return View(chungLoai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminChungLoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("sua-chung-loai/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Edit(ChungLoai chungLoai)
        {
            try
            {
                int d = db.ChungLoais.Count(p => p.ID != chungLoai.ID && p.MaCL == chungLoai.MaCL);
                if (d > 0) ModelState.AddModelError("MaCL", $"Mã chủng loại {chungLoai.MaCL} bị trùng.");
                if (ModelState.IsValid)
                {
                    db.Entry(chungLoai).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
   
                return View(chungLoai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi ghi dữ liệu không thành công.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Xóa chủng loại
        // GET: AdminChungLoai/Delete/5
        [Route("xoa-chung-loai/{id?}")]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                ChungLoai chungLoai = await db.ChungLoais.FindAsync(id);
                if (chungLoai == null) throw new Exception($"Loai ID={id} không tồn tại.");
                return View(chungLoai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminChungLoai/Delete/5
        [Route("xoa-chung-loai/{id?}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                ChungLoai chungLoai = await db.ChungLoais.FindAsync(id);
                db.ChungLoais.Remove(chungLoai);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                int d = await db.Loais.CountAsync(p => p.ChungLoaiID == id);
                object cauBaoLoi;
                if (d > 0)
                    cauBaoLoi = $"Không xóa được chủng loại ID={id}, vì đã có {d} loại hàng liên quan.";
                else
                    cauBaoLoi = $"Lỗi Xóa dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Search chủng loại theo Mã hoặc Tên
        [Route]
        [Authorize]
        public async Task<ActionResult> Search(string search)
        {
            try
            {
                if (!String.IsNullOrEmpty(search))
                {

                    ChungLoai chungLoai = await db.ChungLoais
                                           .FirstOrDefaultAsync(p => p.TenCL.Contains(search) || p.MaCL.Contains(search));

                    if (chungLoai == null) return View("BaoLoi", model: $"Chủng loại tìm kiếm:{search} không tồn tại!");

                    var chungLoais = await db.ChungLoais
                                           .Where(p => p.TenCL.Contains(search) || p.MaCL.Contains(search))
                                           .ToListAsync();                 
                    return View("Index", chungLoais);
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
