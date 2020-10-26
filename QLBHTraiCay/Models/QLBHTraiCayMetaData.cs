using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBHTraiCay.Models
{
    [MetadataType(typeof(HoaDon.HoaDonMetaData))]
    public partial class HoaDon
    {
        class HoaDonMetaData
        {
            [Display(Name = "ID")]
            public int ID;

            [Display(Name = "Ngày đặt hàng")]
            public System.DateTime NgayDatHang;

            [Display(Name = "Họ tên khách hàng")]
            [MaxLength(50, ErrorMessage = "{0} tối đa là {1} ký tự.")]
            public string HoTenKhach;

            [Display(Name = "Địa chỉ")]
            [MaxLength(150, ErrorMessage = "{0} tối đa là {1} ký tự.")]
            public string DiaChi;

            [Display(Name = "Điện thoại")]
            [MaxLength(30, ErrorMessage = "{0} tối đa là {1} ký tự.")]
            public string DienThoai;

            [Display(Name = "Email")]
            [MaxLength(50, ErrorMessage = "{0} tối đa là {1} ký tự.")]
            [EmailAddress(ErrorMessage = "{0} không hợp chuẩn.")]
            public string Email;

            [Display(Name = "Tổng tiền")]
            [DisplayFormat(DataFormatString = "{0:#,##0VND}")]
            public int TongTien;
        }
    }

    [MetadataType(typeof(HangHoa.HangHoaMetaData))]
    public partial class HangHoa
    {
        class HangHoaMetaData
        {
            [Display(Name = "ID")]
            public int ID;

            [Display(Name = "Mã số")]
            [Required(ErrorMessage = "{0} không được để trống.")]
            [MaxLength(10, ErrorMessage = "{0} tối đa là {1} ký tự")]
            [MinLength(2, ErrorMessage = "{0} tối thiểu là {1} ký tự")]
            [Remote("TrungMaSoHangHoa", "KiemTraDuLieu", AdditionalFields = "ID")]
            public string MaHang { get; set; }

            [Display(Name = "Tên hàng")]
            [Required(ErrorMessage = "{0} không được để trống.")]
            [MaxLength(100, ErrorMessage = "{0} tối đa là {1} ký tự")]
            public string TenHang;

            [Display(Name = "Đơn vị tính")]
            [MaxLength(50, ErrorMessage = "{0} tối đa là {1} ký tự")]
            public string DVT;

            [Display(Name = "Quy cách")]
            [MaxLength(50, ErrorMessage = "{0} tối đa là {1} ký tự")]
            public string QuyCach;

            [Display(Name = "Mô tả")]
            public string MoTa;

            [Display(Name = "Giá bán")]
            [DisplayFormat(DataFormatString = "{0:#,##0VND}")]
            [Required(ErrorMessage = "{0} không được để trống.")]
            [RegularExpression(@"\d*", ErrorMessage = "{0} Phải nhập số nguyên >=0.")]
            [Range(0, int.MaxValue, ErrorMessage = "{0} phải từ {1} đến {2}")]
            public int GiaBan;

            [Display(Name = "Giá thị trường")]
            [DisplayFormat(DataFormatString = "{0:#,##0VND}")]
            [RegularExpression(@"\d*", ErrorMessage = "{0} Phải nhập số nguyên >=0.")]
            [Range(0, int.MaxValue, ErrorMessage = "{0} phải từ {1} đến {2}")]
            public Nullable<int> GiaThiTruong;

            [Display(Name = "Loại")]
            [Range(1, int.MaxValue, ErrorMessage = "Phải chọn {0} cho mặt hàng.")]
            public int LoaiID;

            //[Display(Name = "Ngày tạo")]
            //[DisplayFormat(DataFormatString = "0:dd/MM/yyyy")]
            //public System.DateTime NgayTao;

            //[Display(Name = "Ngày sửa")]
            //[DisplayFormat(DataFormatString = "0:dd/MM/yyyy")]
            //public System.DateTime NgaySua;

            [Display(Name = "Xuất xứ")]
            public string XuatXu;

            [Display(Name = "Tình Trạng")]
            [Required(ErrorMessage = "{0} không được để trống.")]
            public int TinhTrang;
        }
    }

    [MetadataType(typeof(Loai.LoaiMetadata))]
    public partial class Loai
    {
        class LoaiMetadata
        {
            [Display(Name = "ID")]
            public int ID;

            [Display(Name = "Mã số")]
            [Required(ErrorMessage = "{0} không được để trống.")]
            [RegularExpression(@"[A-Z]{2,4}", ErrorMessage = "{0} phải nhập từ 2 đến 4 ký tự in hoa [A-Z].")]
            public string MaLoai;

            [Display(Name = "Tên loại")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [MaxLength(100, ErrorMessage = "{0} phải nhập tối đa là {1} ký tự.")]
            public string TenLoai;

            [Display(Name = "Chủng loại")]
            public Nullable<int> ChungLoaiID;
        }
    }

    [MetadataType(typeof(ChungLoai.ChungLoaiMetadata))]
    public partial class ChungLoai
    {
        class ChungLoaiMetadata
        {
            [Display(Name = "ID")]
            public int ID;

            [Display(Name = "Mã loại")]
            [Required(ErrorMessage = "{0} không được để trống.")]
            [RegularExpression(@"[A-Z]{2,4}", ErrorMessage = "{0} phải nhập từ 2 đến 4 ký tự in hoa [A-Z].")]
            public string MaCL;

            [Display(Name = "Tên chủng loại")]
            [Required(ErrorMessage = "{0} không được để trống")]
            [MaxLength(100, ErrorMessage = "{0} phải nhập tối đa là {1} ký tự.")]
            public string TenCL;
        }
    }

}