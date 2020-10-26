using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTraiCay.Models
{
    public class GioHangItem
    {
        public HangHoa HangHoa { get; set; }
        public int SoLuong { get; set; }

        public GioHangItem()
        {
        }

        public GioHangItem(HangHoa hangHoa, int soLuong)
        {
            this.HangHoa = hangHoa;
            this.SoLuong = soLuong;
        }
    }

    public class GioHangModel
    {
        private List<GioHangItem> _DanhSach = new List<GioHangItem>();
        
        public List<GioHangItem> DanhSach => _DanhSach;

        
        public void Them(GioHangItem item)
        {
            var gioHangItem = _DanhSach.Find(p => p.HangHoa.ID == item.HangHoa.ID);
            if (gioHangItem == null)
                _DanhSach.Add(item);
            else
                gioHangItem.SoLuong += item.SoLuong;
        }
        public void HieuChinh(int id, int SoLuong)
        {
            var itemHieuChinh = _DanhSach.Find(p => p.HangHoa.ID == id);
            itemHieuChinh.SoLuong = SoLuong;
        }
        public void Xoa(int id)
        {
            var itemXoa = _DanhSach.Find(p => p.HangHoa.ID == id);
            _DanhSach.Remove(itemXoa);
        }
        public void XoaTatCa()
        {
            _DanhSach.Clear();
        }

       
        public int TongSanPham
        {
            get { return _DanhSach.Count; }
        }
        public int TongSoLuong
        {
            get { return _DanhSach.Sum(p => p.SoLuong); }
        }
        public int TongTriGia
        {
            get
            {
                int kq = 0;
                kq = _DanhSach.Sum(p => p.SoLuong * p.HangHoa.GiaBan);
                return kq;
            }
        }
    }
}