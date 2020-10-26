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
using System.IO;
using X.PagedList;

namespace QLBHTraiCay.Controllers
{
    [RoutePrefix("quan-ly/hang-hoa")]
    [Authorize]
    public class AdminHangHoaController : Controller
    {
        private QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();

        #region Index Ajax WebGrid
        // GET: AdminHangHoa
        [Route("danh-sach-hang-hoa")]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            try
            { 
                var hangHoas =  await db.HangHoas
                                        .Include(h => h.Loai)
                                        .ToListAsync();
           
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_IndexPartial", hangHoas);
                }
                return View(hangHoas);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Chi tiết hàng hóa
        // GET: AdminHangHoa/Details/5
        [Route("chi-tiet-hang-hoa/{id?}")]
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                HangHoa hangHoa = await db.HangHoas
                                          .Include(p=>p.Loai)
                                          .Where(p=>p.ID ==id)
                                          .SingleOrDefaultAsync();
                if (hangHoa == null)
                {
                    return View("BaoLoi", model: $"Không tìm thấy hàng hóa.");
                }
                return View(hangHoa);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Select Danh sách Loại
        // GET:
        [Authorize]
        public async Task<SelectList> TaoDanhSachLoai(int IDChon=0)
        {
            var items = await db.Loais
                                .Select(p => new
                                {
                                    p.ID,
                                    ThongTin = p.MaLoai + " - " + p.TenLoai
                                })
                                .ToListAsync();

            items.Insert(0, new {ID=0, ThongTin="------ Chọn loại ------" });
            var result  = new SelectList(items, "ID", "ThongTin", selectedValue: IDChon);
            return result;
        }
        #endregion

        #region Thêm Hàng hóa
        // GET: AdminHangHoa/Create
        [Route("them-moi-hang-hoa")]
        [Authorize]
        public async Task<ActionResult> Create()
        {
            try
            {
                ViewBag.LoaiID = await TaoDanhSachLoai();
                return View();
            }
            catch(Exception ex)
            {
                return View("BaoLoi", model: $"Không truy cập được dữ liệu. {ex.Message}");
            }
        }

        // POST: AdminHangHoa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("them-moi-hang-hoa")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create(HangHoa hangHoa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    hangHoa.NgayTao = DateTime.Today;
                    hangHoa.NgaySua = DateTime.Today;

                    db.HangHoas.Add(hangHoa);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                ViewBag.LoaiID = await TaoDanhSachLoai(hangHoa.LoaiID);
                return View(hangHoa);
            }
            catch(Exception ex)
            {
                return View("BaoLoi",model: $"Không ghi được dữ liệu. {ex.Message}");
            }            
        }
        #endregion

        #region Sửa hàng hóa
        // GET: AdminHangHoa/Edit/5
        [Route("sua-hang-hoa/{id?}")]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                HangHoa hangHoa = await db.HangHoas.FindAsync(id);
                if (hangHoa == null) throw new Exception($"Hàng hóa ID={id} không tồn tại.");

                ViewBag.LoaiID = await TaoDanhSachLoai(hangHoa.LoaiID);              
                hangHoa.TenHinh = null;

                return View(hangHoa);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminHangHoa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("sua-hang-hoa/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Edit([Bind(Include = "ID,MaHang,TenHang,DVT,QuyCach,GiaBan,GiaThiTruong,LoaiID,XuatXu,TinhTrang")] HangHoa hangHoa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HangHoa hangHoaEdit = await db.HangHoas.FindAsync(hangHoa.ID);
                    TryUpdateModel(hangHoaEdit, new string[] { "ID","MaHang","TenHang","DVT","QuyCach","GiaBan","GiaThiTruong","LoaiID","XuatXu","TinhTrang" });
                    hangHoaEdit.NgaySua = DateTime.Today;
                    //db.Entry(hangHoa).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
       
                //ViewBag.LoaiID = new SelectList(db.Loais, "ID", "MaLoai", hangHoa.LoaiID);
                ViewBag.LoaiID = await TaoDanhSachLoai(hangHoa.LoaiID);
                return View(hangHoa);
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Không ghi được dữ liệu. <br/> Lý do:{ex.Message}");
            }
        }
        #endregion

        #region Xóa hàng hóa
        // GET: AdminHangHoa/Delete/5
        [Route("xoa-hang-hoa/{id?}")]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                HangHoa hangHoa = await db.HangHoas
                                    .Include(p => p.Loai)
                                    .SingleOrDefaultAsync(p => p.ID == id);
                if (hangHoa == null) throw new Exception($"Hàng hóa ID={id} không tồn tại.");
                return View(hangHoa);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }

        // POST: AdminHangHoa/Delete/5
        [Route("xoa-hang-hoa/{id?}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "QuanLy")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                HangHoa hangHoa = await db.HangHoas.FindAsync(id);
                db.HangHoas.Remove(hangHoa);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                int d = await db.HoaDonChiTiets.CountAsync(p => p.HangHoaID == id);
                object cauBaoLoi;
                if (d > 0)
                    cauBaoLoi = $"Không xóa được hàng hóa ID={id}, vì đã có {d} hóa đơn liên quan.";
                else
                    cauBaoLoi = $"Lỗi Xóa dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Search hàng hóa theo Mã hoặc Tên
        [Route]
        [Authorize]
        public async Task<ActionResult> Search(string search)
        {
            try
            {
                if (!String.IsNullOrEmpty(search))
                {
                    HangHoa hangHoa = await db.HangHoas
                                        .FirstOrDefaultAsync(p => p.TenHang.Contains(search) || p.MaHang.Contains(search));
                    if (hangHoa == null) return View("BaoLoi", model: $"Hàng hóa tìm kiếm:{search} không tồn tại!");

                    var hangHoas = await db.HangHoas
                                           .Include(p => p.Loai)
                                           .Where(p => p.TenHang.Contains(search) || p.MaHang.Contains(search))
                                           .ToListAsync();

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_IndexPartial", hangHoas);
                    }
                    return View("Index", hangHoas);
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

        #region Giai phóng biến
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Xử lý Upload file hình
        //GET : Admin/HangHoa/Upload/5
        [Route("upload-hinh-hang-hoa/{id?}")]
        [Authorize]
        public async Task<ActionResult> Upload(int? id)
        {
            if (id == null || id < 1) return RedirectToAction("Index");
            try
            {
                HangHoa item = await db.HangHoas.FindAsync(id);
                if (item == null) throw new Exception("ID:" + id + "không tồn tại");
                return View(item);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = "không truy cập được dữ liệu <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }
        }
        //POST :Admin/HangHoa/Upload/5
        [Route("upload-hinh-hang-hoa/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Upload(int id, HttpPostedFileBase[] tapTins)
        {
            try
            {
                HangHoa item = await db.HangHoas.FindAsync(id);
                if (tapTins[0] != null)
                {
                    string duongDan = Server.MapPath("~/Photos/");
                    string dsTen = null;                    
                    if (!string.IsNullOrEmpty(item.TenHinh))
                    {
                        foreach (var tenHinh in item.Hinhs)
                        {
                            string pathAndFname = Server.MapPath($"~/Photos/{tenHinh}");
                            if (System.IO.File.Exists(pathAndFname))
                                System.IO.File.Delete(pathAndFname);
                        }
                    }
                  
                    for (int i = 0; i < tapTins.Length; i++)
                    {
                        var f = tapTins[i];
                        string kieu = Path.GetExtension(f.FileName);
                        string ten = $"{id}-{i + 1}{kieu}";
                        f.SaveAs(duongDan + ten);
                        dsTen += $"{ten},";
                    }

                    item.TenHinh = dsTen.TrimEnd(',');
                    await db.SaveChangesAsync();
                    
                    return RedirectToAction("Index");
                }
                // Trường hợp chưa chọn file hoặc file không có nội dung thì quay trở lại view upload
                ViewBag.ThongBao = "Bạn chưa chọn file hoặc file bạn chọn không có nội dung.";
                return View(item);
            }
            catch (Exception ex)
            {
                object cauBaoLoi = "Upload không thành công <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion
  
    }
}
