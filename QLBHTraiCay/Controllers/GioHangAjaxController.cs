using QLBHTraiCay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace QLBHTraiCay.Controllers
{
    public class GioHangAjaxController : Controller
    {
        QLBHTraiCayDbContext db = new QLBHTraiCayDbContext();

        #region Giỏ hàng index
        // GET: GioHangAjax
        [Route("gio-hang")]
        public ActionResult Index()
        {
            try
            {
                //Tham chiếu đến giỏ hàng lưu trong Session
                var gioHang = Session["GioHang"] as GioHangModel;
                ViewBag.ShoppingCartAct = "active";
                ViewBag.GioHang = gioHang;
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_IndexPartial");
                }
                return View();
            }
            catch(Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }

        }
        #endregion

        #region Thêm Vào giỏ hàng
        // POST: GioHangAjax/Create
        [HttpPost]
        [Route("gio-hang-them")]
        public ActionResult AddToCart(int HangHoaID, int SoLuong = 1)
        {
            try
            {
                //Tham chiếu đến giỏ hàng lưu trong Session
                var gioHang = Session["GioHang"] as GioHangModel;
                if (gioHang == null)
                {//Lần đầu chọn mua 1 mã hàng
                    gioHang = new GioHangModel();
                    Session["GioHang"] = gioHang;
                }
                HangHoa hangHoa = db.HangHoas.Find(HangHoaID);
                var item = new GioHangItem(hangHoa, SoLuong);
                gioHang.Them(item);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }

        }
        #endregion

        #region Sửa giỏ hàng
        // POST: GioHangAjax/Edit/5
        [HttpPost]
        [Route("gio-hang-sua")]
        public ActionResult Edit(int HangHoaID, int SoLuong)
        {
            try
            {
                //Tham chiếu đến giỏ hàng trong Session
                var gioHang = Session["GioHang"] as GioHangModel;
                gioHang.HieuChinh(HangHoaID, SoLuong);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }
        }
        #endregion

        #region Xóa giỏ hàng
        // POST: GioHangAjax/Delete/5
        [HttpPost]
        [Route("gio-hang-xoa")]
        public ActionResult Delete(int HangHoaID)
        {
            try
            {
                //Tham chiếu đến giỏ hàng trong Session
                var gioHang = Session["GioHang"] as GioHangModel;
                gioHang.Xoa(HangHoaID);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("BaoLoi", model: $"Lỗi truy cập dữ liệu.<br>{ex.Message}");
            }
        }
        #endregion

        #region Xử lý đơn đặt hàng
        [HttpGet]
        [Route("dat-hang")]
        public ActionResult DatHang()
        {
            return View();
        }

        [HttpPost]
        [Route("dat-hang")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DatHang(HoaDon hoaDon)
        {
            var gioHang = Session["GioHang"] as GioHangModel;
            if (gioHang == null || gioHang.TongSanPham == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //1.Thêm HoaDon
                hoaDon.NgayDatHang = DateTime.Now;
                hoaDon.TongTien = gioHang.TongTriGia;
                db.HoaDons.Add(hoaDon);
                //2.Thêm HoaDonChiTiet
                foreach (var item in gioHang.DanhSach)
                {
                    HoaDonChiTiet ct = new HoaDonChiTiet();
                    ct.HoaDonID = hoaDon.ID;
                    ct.HangHoaID = item.HangHoa.ID;
                    ct.SoLuong = item.SoLuong;
                    ct.DonGia = item.HangHoa.GiaBan;
                    ct.ThanhTien = item.HangHoa.GiaBan * item.SoLuong;
                    db.HoaDonChiTiets.Add(ct);
                }
                await db.SaveChangesAsync();
               
                gioHang.XoaTatCa();

                var hoaDonCTs = await db.HoaDonChiTiets
                                        .Include(p => p.HoaDon)
                                        .Include(p => p.HangHoa)
                                        .Where(p => p.HoaDonID == hoaDon.ID)
                                        .ToListAsync();
                ViewBag.hoaDonCT = hoaDonCTs;
                return View("DatHangThanhCong");
            }
            catch (Exception ex)
            {
                TempData["LoiDatHang"] = "Đặt hàng không thành công.<br>" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        #endregion
    }
}