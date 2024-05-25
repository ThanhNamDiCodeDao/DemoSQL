using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoSQL.Data
{
    [Table("Loai")]
    public class Loai
    {
        [Key]
        public int MaLoai {  get; set; }
        [Required]
        [MaxLength(50)]
        public string LoaiName { get; set;}

        public virtual ICollection<HangHoa> HangHoas { get; set;}
}
}
