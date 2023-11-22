using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LB2.Models.ViewModels
{
    public class StudentVM
    {
        [Key]
        public System.Guid ID_студента { get; set; }
        [Required]
        [DisplayName("Фамилия студента")]
        [StringLength(100, MinimumLength = 2)]
        public string Фамилия { get; set; }
        [Required]
        [DisplayName("Имя студента")]
        public string Имя { get; set; }
        [DisplayName("Отчество студента")]
        public string Отчество { get; set; }

        public bool Пол { get; set; }
        public string Адрес_проживания { get; set; }
        [Required]
        [DisplayName("Дата рождения студента")]
        public string Дата_рождения { get; set; }
        public string Уровень_владения_ИЯ { get; set; }
        
    }
}