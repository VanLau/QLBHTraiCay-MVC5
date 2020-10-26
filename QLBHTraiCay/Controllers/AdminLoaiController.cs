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

using X.PagedList;

namespace QLBHTraiCay.Controllers
{
    [RoutePrefix("quan-ly/loai-hang")]
    [Authorize]
    public class AdminLoaiController : Controller
    {
        private QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();

        #region Danh sách Loại ajax WebGrid 
        // GET: AdminLoai
        [Route("danh-sach-loai-hang")]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            try
            {
                var loais = await db.Loais
                            .Include(l => l.ChungLoai)
                            .ToListAsync();
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_IndexPartial", loais);
                }
                return View(loais);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = "không truy cập được dữ liệu <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Chi tiết loại hàng
        // GET: AdminLoai/Details/5
        [Route("chi-tiet-loai-hang/{id?}")]
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                Loai loai = await db.Loais
                                    .Include(p=>p.ChungLoai)
                                    .Where(p => p.ID==id)
                                    .SingleOrDefaultAsync();
                if (loai == null)
                {
                    return View("BaoLoi", model: $"Không tìm thấy loại hàng.");
                }
                return View(loai);
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Không truy cập được dữ liệu. {ex.Message}");
            }
        }
        #endregion

        #region Tạo danh sách chủng loại
        [Authorize]
        public async Task<SelectList> TaoDanhSachCL(int IDChon = 0)
        {
            var items = await db.ChungLoais
                                .Select(p => new
                                {
                                    p.ID,
                                    ThongTin = p.TenCL
                                })
                                .ToListAsync();

            items.Insert(0, new { ID = 0, ThongTin = "------ Chọn loại ------" });
            var result = new SelectList(items, "ID", "ThongTin", selectedValue: IDChon);
            return result;
        }
        #endregion

        #region Thêm loại hàng - MVC
        // GET: AdminLoai/Create
        [Route("them-moi-loai-hang")]
        [Authorize]
        public async Task<ActionResult> Create()
        {
            try
            {
                ViewBag.ChungLoaiID = await TaoDanhSachCL();
                return View();
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Không truy cập được dữ liệu. {ex.Message}");
            }
        }

        // POST: AdminLoai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("them-moi-loai-hang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(Loai loai)
        {
            try
            {
                int d = db.Loais.Count(p => p.MaLoai == loai.MaLoai);
                if (d > 0) ModelState.AddModelError("MaLoai", $"Mã số {loai.MaLoai} bị trùng.");
                if (ModelState.IsValid)
                {
                    db.Loais.Add(loai);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                
                ViewBag.ChungLoaiID = await TaoDanhSachCL(loai.ChungLoaiID.Value);
                return View(loai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi ghi dữ liệu không thành công.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Sửa loại hàng
        // GET: AdminLoai/Edit/5
        [Route("sua-loai-hang/{id?}")]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                Loai loai = await db.Loais.FindAsync(id);
                if (loai == null) throw new Exception($"Loại ID={id} không tồn tại.");

                ViewBag.ChungLoaiID = await TaoDanhSachCL(loai.ChungLoaiID.Value);
                return View(loai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminLoai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("sua-loai-hang/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Edit(Loai loai)
        {
            try
            {
                int d = db.Loais.Count(p => p.ID != loai.ID && p.MaLoai == loai.MaLoai);
                if (d > 0) ModelState.AddModelError("MaLoai", $"Mã loại {loai.MaLoai} bị trùng.");
                if (ModelState.IsValid)
                {
                    db.Entry(loai).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }              
                ViewBag.ChungLoaiID = await TaoDanhSachCL(loai.ChungLoaiID.Value);
                return View(loai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi ghi dữ liệu không thành công.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Xóa loại hàng
        // GET: AdminLoai/Delete/5
        [Route("xoa-loai-hang/{id?}")]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                Loai loai = await db.Loais
                                    .Include(p => p.ChungLoai)
                                    .SingleOrDefaultAsync(p => p.ID == id);
                if (loai == null) throw new Exception($"Loai ID={id} không tồn tại.");                
                return View(loai);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminLoai/Delete/5
        [Route("xoa-loai-hang/{id?}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Loai loai = await db.Loais.FindAsync(id);
                db.Loais.Remove(loai);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                int d = await db.HangHoas.CountAsync(p => p.LoaiID == id);
                object cauBaoLoi;
                if (d > 0)
                    cauBaoLoi = $"Không xóa được loại ID={id}, vì đã có {d} mặt hàng liên quan.";
                else
                    cauBaoLoi = $"Lỗi Xóa dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Search loại theo Mã hoặc Tên
        [Route]
        [Authorize]
        public async Task<ActionResult> Search(string search)
        {
            try
            {
                if (!String.IsNullOrEmpty(search))
                {
                    Loai loai = await db.Loais
                                        .FirstOrDefaultAsync(p => p.TenLoai.Contains(search)|| p.MaLoai.Contains(search));
                    if (loai == null) return View("BaoLoi", model: $"Loại tìm kiếm:{search} không tồn tại!");

                    var loais = await db.Loais
                                        .Include(p => p.ChungLoai)
                                        .Where(p => p.TenLoai.Contains(search) || p.MaLoai.Contains(search))
                                        .ToListAsync();
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_IndexPartial", loais);
                    }
                    return View("Index", loais);
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
