using DemoSQL.Data;
using DemoSQL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoSQL.Services
{
    public class HangHoaRepository : IHangHoaRepository
    {
        private readonly MyDbContext _context;
        public static int PAGE_SIZE { get; set; } = 5;

        public HangHoaRepository(MyDbContext context)
        {
            _context = context;
        }
        public List<HangHoaModel> GetAll(string search, double? from, double? to, string sortBy, int page = 1)
        {
            var allProducts = _context.HangHoas.Include(x => x.Loai).AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                allProducts = allProducts.Where(x => x.TenHangHoa.Contains(search));

            }               

            if(from.HasValue)
            {
                allProducts = allProducts.Where(x => x.DonGia >= from);
            }

            if (to.HasValue)
            {
                allProducts = allProducts.Where(x => x.DonGia <= to);
            }
            #endregion

            #region Sort
            //Default Sort by name (Ten Hang Hoa)
            allProducts = allProducts.OrderBy(x => x.TenHangHoa);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch(sortBy)
                {
                    case "TenHangHoa_desc": 
                        allProducts = allProducts.OrderByDescending(x => x.TenHangHoa);
                        break;
                    case "DonGia_asc":
                        allProducts = allProducts.OrderBy(x => x.DonGia);
                        break;
                    case "DonGia_desc":
                        allProducts = allProducts.OrderByDescending(x => x.DonGia);
                        break;
                }
            }
            #endregion

            //#region Paging
            //allProducts = allProducts.Skip((page-1)*PAGE_SIZE).Take(PAGE_SIZE);
            //#endregion
            //var result = allProducts.Select(x => new HangHoaModel
            //{
            //    MaHangHoa = x.MaHangHoa,
            //    TenHangHoa = x.TenHangHoa,
            //    DonGia = x.DonGia,
            //    TenLoai = x.Loai.LoaiName,
            //});
            //return result.ToList();

            var result = PaginatedList<Data.HangHoa>.Create(allProducts, page, PAGE_SIZE);

            return result.Select(x => new HangHoaModel
            {
                MaHangHoa = x.MaHangHoa,
                TenHangHoa = x.TenHangHoa,
                DonGia = x.DonGia,
                TenLoai = x.Loai?.LoaiName,
            }).ToList();
        }
    }
}
