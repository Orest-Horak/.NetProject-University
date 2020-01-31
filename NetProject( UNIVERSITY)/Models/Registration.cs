using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace NetProject__UNIVERSITY_.Models
{
    public class Registration
{
        [Required(ErrorMessage = "Please enter a First Name")]
        [RegularExpression(@"^[a-zA-Z]{1,40}$",
        ErrorMessage = "Characters are not allowed.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a Last Name")]
        [RegularExpression(@"^[a-zA-Z]{1,40}$",
         ErrorMessage = "Characters are not allowed.")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Please enter a Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Please enter a phone number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your e-mail")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [RegularExpression(@"^[a-zA-Z0-9]{10,20}$",
         ErrorMessage = "Your password is too weak !")]
        public string Password { get; set; }


    }
}
