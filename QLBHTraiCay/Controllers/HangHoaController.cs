

using QLBHTraiCay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;


namespace QLBHTraiCay.Controllers
{
    public class HangHoaController : Controller
    {
        
        QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();
        #region Hang Hóa Trong Nước
        //GET: HangHoaTN  
        [Route()]
        public async Task<ActionResult> HangHoaIndex(int? page, int? CLID)
        {
            try
            {
                if (CLID == null) CLID = 1;
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 12;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.HangHoas
                                             .Where(p => p.Loai.ChungLoaiID == CLID)
                                             .CountAsync();
                var onePageOfData = await db.HangHoas
                                            .Where(p => p.Loai.ChungLoaiID == CLID)
                                            .OrderBy(p => p.ID)
                                            .Skip(n)
                                            .Take(pageSize)
                                            .ToListAsync();
                ViewBag.OnePageOfData = new StaticPagedList<HangHoa>(onePageOfData, pageNumber, pageSize, totalItemCount);

                ViewBag.TieuDe = "TRÁI CÂY TRONG NƯỚC";
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_HangHoaTNAjaxPartial");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }

        }
        #endregion

        #region Hang Hóa Nhập Khẩu
        // GET: HangHoaNK
        [ChildActionOnly]
        public PartialViewResult _HangHoaNKPartial(int? CLID)
        {
            try
            {
                if (CLID == null) CLID = 2;
                var hhs = db.HangHoas
                            .Where(p => p.Loai.ChungLoaiID == CLID)
                            .ToList();
                ViewBag.HangHoas = hhs;
                ViewBag.TieuDe = "TRÁI CÂY NHẬP KHẨU";
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }
        }
        #endregion

        #region Shop
        [Route("danh-sach-hang-hoa")]
        public async Task<ActionResult> ShopListAjax(int? page)
        {
            try
            {
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 12;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.HangHoas.CountAsync();
                var onePageOfData = await db.HangHoas
                                            .OrderBy(p => p.ID)
                                            .Skip(n)
                                            .Take(pageSize)
                                            .ToListAsync();
                ViewBag.OnePageOfData = new StaticPagedList<HangHoa>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.TieuDe = "SẢN PHẨM";

                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ShopListAjaxPartial");
                }
                return View();
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }
        }
        #endregion

        #region Tra cứu theo Loại
        [Route("hang-hoa/theo-loai/{id?}")]
        public async Task<ActionResult> TraCuuTheoLoaiAjax(int? id, int? page)
        {
            if (id == null || id < 1) return RedirectToAction("HangHoaIndex", "HangHoa");
            try
            {
                Loai LoaiItem = await db.Loais
                                  .Include(p=>p.ChungLoai)
                                  .Where(p=>p.ID==id)
                                  .SingleOrDefaultAsync();
                if (LoaiItem == null) return View("BaoLoi", model: $"Loai {id} không tồn tại!");
                
                int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                int pageSize = 6;
                int n = (pageNumber - 1) * pageSize;
                int totalItemCount = await db.HangHoas
                                             .Where(p => p.LoaiID == id)
                                             .CountAsync();
                var onePageOfData = await db.HangHoas
                                            .Where(p => p.LoaiID == id)
                                            .OrderBy(p => p.ID)
                                            .Skip(n)
                                            .Take(pageSize)
                                            .ToListAsync();
                ViewBag.OnePageOfData = new StaticPagedList<HangHoa>(onePageOfData, pageNumber, pageSize, totalItemCount);
                ViewBag.LoaiID = id;
                ViewBag.page = totalItemCount;
                ViewBag.pageSize = pageSize;
                ViewBag.TieuDe = $"{LoaiItem.ChungLoai.TenCL} - {LoaiItem.TenLoai}";
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ShopListAjaxPartial");
                }
                return View("ShopListAjax");
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }

        }
        #endregion

        #region Chi tiết sản phẩm   
        [Route("san-pham/{name}-{id?}")]
        public async Task<ActionResult> ChiTietSanPham(int? id, string name)
        {
            if (id == null || id < 1) return RedirectToAction("HangHoaIndex", "HangHoa");
            try
            {
                HangHoa hangHoa = await db.HangHoas
                                          .Include("Loai.ChungLoai")
                                          .SingleOrDefaultAsync(p => p.ID == id);
                if (hangHoa == null) throw new Exception($"Mặt hàng ID ={id} không tồn tại.");
                return View(hangHoa); 
            }
            catch (Exception ex)
            {
                object cauBaoLoi = $"Lỗi truy cập dữ liệu.<br/>lý do: {ex.Message}";
                return View("BaoLoi", cauBaoLoi);
            }
        }
        #endregion

        #region Sản phẩm bán chạy
        [ChildActionOnly]
        public PartialViewResult _HangHoaBCPartial()
        {
            try
            {
                var rs = db.HangHoas
                           .Join(db.HoaDonChiTiets,
                                    emp => emp.ID, per => per.HangHoaID,
                                    (emp, per) => new
                                    {
                                        emp.ID,
                                        emp.MaHang,
                                        emp.TenHang,
                                        emp.DVT,
                                        emp.QuyCach,
                                        emp.MoTa,
                                        emp.TenHinh,
                                        emp.GiaBan,
                                        emp.GiaThiTruong,
                                        emp.LoaiID,
                                        emp.NgayTao,
                                        emp.NgaySua,
                                        per.SoLuong
                                    })
                           .GroupBy(x => x.ID)
                           .Select(a => new
                           {
                               ID = a.Max(b => b.ID),
                               MaHang = a.Max(b => b.MaHang),
                               TenHang = a.Max(b => b.TenHang),
                               DVT = a.Max(b => b.DVT),
                               QuyCach = a.Max(b => b.QuyCach),
                               MoTa = a.Max(b => b.MoTa),
                               TenHinh = a.Max(b => b.TenHinh),
                               GiaBan = a.Max(b => b.GiaBan),
                               GiaThiTruong = a.Max(b => b.GiaThiTruong),
                               LoaiID = a.Max(b => b.LoaiID),
                               NgayTao = a.Max(b => b.NgayTao),
                               NgaySua = a.Max(b => b.NgaySua),
                               SoLuong = a.Sum(b => b.SoLuong)
                           })
                          .OrderByDescending(a => a.SoLuong)
                          .Take(7)
                          .ToList();

                List<HangHoa> hhs = new List<HangHoa>();
                foreach (var item in rs)
                {
                    HangHoa hh = new HangHoa();
                    hh.ID = item.ID;
                    hh.MaHang = item.MaHang;
                    hh.TenHang = item.TenHang;
                    hh.DVT = item.DVT;
                    hh.QuyCach = item.QuyCach;
                    hh.MoTa = item.MoTa;
                    hh.TenHinh = item.TenHinh;
                    hh.GiaBan = item.GiaBan;
                    hh.GiaThiTruong = item.GiaThiTruong;
                    hh.LoaiID = item.LoaiID;
                    hh.NgayTao = item.NgayTao;
                    hh.NgaySua = item.NgaySua;
                    hhs.Add(hh);
                }
                ViewBag.HangHoas = hhs;
                return PartialView();
            }
            catch (Exception ex)
            {
                return PartialView($"Lỗi truy cập dữ liệu.< br /> lý do: { ex.Message}");
            }
        }
        #endregion

        #region Search hàng hóa theo tên
        
        [HttpPost]
        [Route("tim-kiem-hang-hoa")]
        public async Task<ActionResult> Search(int? page, string search)
        { 
            try
            {
                if (!String.IsNullOrEmpty(search))
                {
                    HangHoa hangHoas = await db.HangHoas
                                           .FirstOrDefaultAsync(p => p.TenHang.Contains(search));
                                           
                    //if(hangHoas==null) return View("BaoLoi", model: $"Hàng hóa tìm kiếm:{search} không tồn tại!");

                    int pageNumber = (page == null || page < 1) ? 1 : page.Value;
                    int pageSize = 4;
                    int n = (pageNumber - 1) * pageSize;
                    int totalItemCount = await db.HangHoas
                                                 .Where(p => p.TenHang.Contains(search))
                                                 .CountAsync();
                    var onePageOfData = await db.HangHoas
                                                .Where(p => p.TenHang.Contains(search))
                                                .OrderBy(p => p.ID)
                                                .Skip(n)
                                                .Take(pageSize)
                                                .ToListAsync();
                    ViewBag.OnePageOfData = new StaticPagedList<HangHoa>(onePageOfData, pageNumber, pageSize, totalItemCount);
                    ViewBag.TieuDe = $"Kết quả tìm kiếm: {search}";
                    ViewBag.page = totalItemCount;
                    ViewBag.pageSize = pageSize;
                    ViewBag.search = search;
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_SearchAjaxPartial");
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("HangHoaIndex");
                }

            }
            catch (Exception ex)
            {
                object cauBaoLoi = "không truy cập được dữ liệu <br/>" + ex.Message;
                return View("BaoLoi", cauBaoLoi);
            }

        }
        #endregion
    }
}