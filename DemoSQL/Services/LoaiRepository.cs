using DemoSQL.Data;
using DemoSQL.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoSQL.Services
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly MyDbContext _context;

        public LoaiRepository(MyDbContext context)
        {
            _context = context;
        }
        public LoaiVM Add(LoaiModel loaiModel)
        {
            var loai = new Loai()
            {
                LoaiName = loaiModel.LoaiName,

            };
            _context.Add(loai);
            _context.SaveChanges();

            return new LoaiVM
            {
                MaLoai = loai.MaLoai,
                LoaiName = loai.LoaiName,
            };
        }

        public void Delete(int id)
        {
            var loai = _context.Loais.FirstOrDefault(x => x.MaLoai == id);
            if (loai != null)
            {
                _context.Remove(loai);
                _context.SaveChanges();
            }
        }

        public List<LoaiVM> GetAll()
        {
            var loais = _context.Loais.Select(x => new LoaiVM
            {
                MaLoai = x.MaLoai,
                LoaiName = x.LoaiName,
            });

            return loais.ToList();
        }

        public LoaiVM GetById(int id)
        {
            var loai = _context.Loais.FirstOrDefault(x => x.MaLoai == id);
            if(loai == null)return null;
            return new LoaiVM
            {
                MaLoai = loai.MaLoai,
                LoaiName = loai.LoaiName,
            };

        }

        public void Update(LoaiVM loaiModel)
        {
            var loai = _context.Loais.FirstOrDefault(x => x.MaLoai == loaiModel.MaLoai);
            if (loai != null)
            {
                _context.Update(loai);
                _context.SaveChanges();
            }
        }
    }
}
