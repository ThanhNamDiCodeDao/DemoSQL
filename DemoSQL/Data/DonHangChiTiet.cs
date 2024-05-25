using System;

namespace DemoSQL.Data
{
    public class DonHangChiTiet
    {
        public Guid MaHangHoa {  get; set; }
        public Guid MaDonHang { get; set; }

        public int SoLuong {  get; set; }

        public byte GiamGia {  get; set; }

        //relationship
        public DonHang DonHang { get; set; }
        public HangHoa HangHoa { get; set; }


    }
}
