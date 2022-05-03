using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FireAndIce.Models.ViewModels.Users
{
    public class createUserViewModel
    {
        [Required]  
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string fName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string lName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
