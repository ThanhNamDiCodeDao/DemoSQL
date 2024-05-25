using DemoSQL.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoSQL.Data
{
    public class MyDbContext: DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        #region DbSet
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<Loai> Loais { get; set; }
        public DbSet<DonHangChiTiet> DonHangChiTiets { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DonHang>(e =>
            {
                e.ToTable("DonHang");
                e.HasKey(dh => dh.MaDonHang);
                e.Property(dh => dh.NgayDatHang).HasDefaultValueSql("getutcdate()");
                e.Property(dh => dh.NguoiNhanHang).IsRequired().HasMaxLength(120);
            });
            modelBuilder.Entity<DonHangChiTiet>(e =>
            {
                e.ToTable("ChiTietDonHang");
                e.HasKey(e => new {e.MaDonHang, e.MaHangHoa});
                e.HasOne(e => e.DonHang).WithMany(e => e.DonHangChiTiets).HasForeignKey(e => e.MaDonHang).HasConstraintName("FK_DonHangChiTiet_DonHang");

                e.HasOne(e => e.HangHoa).WithMany(e => e.DonHangChiTiets).HasForeignKey(e => e.MaHangHoa).HasConstraintName("FK_DonHangChiTiet_HangHoa");

            });

            modelBuilder.Entity<NguoiDung>(e =>
            {
                e.HasIndex(e => e.Username).IsUnique();
                e.Property(e => e.HovaTen).IsRequired().HasMaxLength(150);
                e.Property(e => e.Email).IsRequired().HasMaxLength(150);
            });
        }
    }
}
