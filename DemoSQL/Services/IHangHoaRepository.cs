using DemoSQL.Models;
using System.Collections.Generic;

namespace DemoSQL.Services
{
    public interface IHangHoaRepository
    {
        List<HangHoaModel>GetAll(string search, double? from, double? to, string sortBy, int page);
    }
}
