using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject__UNIVERSITY_.Models
{
    public class Log_in
    {
        [Required(ErrorMessage = "Please enter Login")]
        [RegularExpression(@"^[a-zA-Z]{1,20}$",
                 ErrorMessage = "Characters are not allowed.")]
        public string Login { get; set; }


        [Required(ErrorMessage = "Please enter a Password")]
        [RegularExpression(@"^[a-zA-Z0-9]{1,30}$",
           ErrorMessage = "Symbols /,*,+,-,!,_  are not allowed.")]
        public string Password { get; set; }

    }
}
