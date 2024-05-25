using DemoSQL.Data;
using DemoSQL.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoSQL.Services
{
    public class LoaiRepositoryInMemory : ILoaiRepository
    {
        static List<LoaiVM> loais = new List<LoaiVM>
        {
            new LoaiVM{MaLoai = 1, LoaiName = "Tivi"},
            new LoaiVM{MaLoai = 2, LoaiName = "Laptop"},
            new LoaiVM{MaLoai = 3, LoaiName = "PC"},
            new LoaiVM{MaLoai = 4, LoaiName = "Tablet"},
        };
        public LoaiVM Add(LoaiModel loai)
        {
            var _loai = new LoaiVM
            {
                LoaiName = loai.LoaiName,
                MaLoai = loais.Max(x => x.MaLoai) + 1,
            };
            loais.Add(_loai);
            return _loai;
        }

        public void Delete(int id)
        {
            var _loai = loais.SingleOrDefault(x => x.MaLoai == id);
            loais.Remove(_loai);
        }

        public List<LoaiVM> GetAll()
        {
            return loais;
        }

        public LoaiVM GetById(int id)
        {
            return loais.SingleOrDefault(x => x.MaLoai == id);
        }

        public void Update(LoaiVM loai)
        {
            var _loai = loais.SingleOrDefault(x => x.MaLoai == loai.MaLoai);
            if(_loai != null)
            {
                _loai.LoaiName = loai.LoaiName;
            }
        }
    }
}
