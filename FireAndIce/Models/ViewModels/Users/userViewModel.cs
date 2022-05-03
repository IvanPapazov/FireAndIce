using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FireAndIce.Models.ViewModels.Users
{
    public class userViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string fName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string lName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
    }
}
