using System;
using System.Collections.Generic;

namespace DemoSQL.Data
{
    public enum TinhTrangDonHang
    {
        New = 0,
        Payment = 1,
        Complete = 2,
        Cancel = -1,
    }
    public class DonHang
    {
        public Guid MaDonHang { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayGiao {  get; set; }
        public TinhTrangDonHang TinhTrangDonHang { get; set; }
        
        public string NguoiNhanHang { get; set; }

        public string DiaChiGiao {  get; set; }

        public string SoDienThoai {  get; set; }

        public ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }

        public DonHang()
        {
            DonHangChiTiets = new HashSet<DonHangChiTiet>();
        }
    }
}
