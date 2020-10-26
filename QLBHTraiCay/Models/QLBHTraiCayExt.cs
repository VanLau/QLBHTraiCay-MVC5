using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBHTraiCay.Models
{
    public partial class HangHoa
    {
        public int Gia1
        {
            get
            {
                int gia1=0;
                if (GiaThiTruong != null)
                {
                    gia1 = int.Parse(GiaThiTruong.ToString());
                }
                else
                {
                    gia1 = 0;
                }
                return gia1;
                
                
            }
            set { }
        }
        public List<string> Hinhs
        {
            get
            {
                var _Hinhs = new List<string>();
                if (!string.IsNullOrEmpty(TenHinh))
                    _Hinhs.AddRange(TenHinh.Split(','));
                else
                    _Hinhs.Add("noImage.jpg");
                return _Hinhs;
            }

            set { }
        }
        public string BiDanh
        {
            get
            {
                string db = XuLyChuoi.LoaiBoDauTiengViet(TenHang);
                return db;
            }
        }
    }

    public partial class Loai
    {
        public string BiDanh
        {
            get
            {
                string db = XuLyChuoi.LoaiBoDauTiengViet(TenLoai);
                return db;
            }
        }
    }
}