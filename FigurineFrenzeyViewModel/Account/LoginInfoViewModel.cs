using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Account
{
    public class LoginInfoViewModel
    {
        [Required]
        [MinLength(10)]
        [MaxLength(11)]
        public string Phone {  get; set; }

        [Required]
        [MinLength(12)]
        [MaxLength(30)]
        public string Password {  get; set; }
    }
}
