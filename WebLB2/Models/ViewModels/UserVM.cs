using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LB2.Models.ViewModels
{
    public class UserVM
    {
        [Required]
        [Key]
        [DisplayName("Логин")]
        public string Login { get;  set; }
        [Required]
        [DisplayName("Пароль")]
        public string Password { get; set; }
    }
}