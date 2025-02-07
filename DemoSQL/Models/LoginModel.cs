﻿using System.ComponentModel.DataAnnotations;

namespace DemoSQL.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [MaxLength(250)]
        public string Password { get; set; }
    }
}
