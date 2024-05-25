using DemoSQL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DemoSQL.Models;
using Microsoft.AspNetCore.Authorization;

namespace DemoSQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiController : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoaiController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var ds_Loai = _context.Loais.ToList();
                return Ok(ds_Loai);
            }
            catch
            {
                return BadRequest();
            }
            
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ds_Loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (ds_Loai == null) return NotFound();
            return Ok(ds_Loai);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateNew(Models.LoaiModel model)
        {
            try
            {
                var loai = new Data.Loai
                {
                    LoaiName = model.LoaiName
                };
                _context.Add(loai);
                _context.SaveChanges();
                return StatusCode(StatusCodes.Status201Created,loai);
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpPut]
        public IActionResult UpdateById(int id, Models.LoaiModel model)
        {
            var loai = _context.Loais.FirstOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();
            loai.LoaiName = model.LoaiName;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteLoaiById (int id)
        {
            var Loai = _context.Loais.SingleOrDefault(x => x.MaLoai == id);
            if (Loai == null) return NotFound();
            _context.Remove(Loai);
            return StatusCode(StatusCodes.Status200OK);
        }
    }
    }
