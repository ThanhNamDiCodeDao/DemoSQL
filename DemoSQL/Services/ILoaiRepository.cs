using DemoSQL.Models;
using System.Collections.Generic;

namespace DemoSQL.Services
{
    public interface ILoaiRepository
    {
        List<LoaiVM> GetAll();
        LoaiVM GetById(int id);
        LoaiVM Add(LoaiModel loai);
        void Update(LoaiVM loai); 
        void Delete(int id);
    }
}
