using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FireAndIce.Models
{
    public class AppUser : IdentityUser
    {
        public string fName { get; set; }
        public string lName { get; set; }
    }
}
