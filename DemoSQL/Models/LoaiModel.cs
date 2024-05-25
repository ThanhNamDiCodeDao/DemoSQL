using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoSQL.Models
{
    [Table("Loai")]
    public class LoaiModel
    {
        [Required]
        [MaxLength(50)]
        public string LoaiName { get; set;}
    }
}
