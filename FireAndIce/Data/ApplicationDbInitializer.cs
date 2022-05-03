using FireAndIce.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FireAndIce.Data
{
    public class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!(roleManager.RoleExistsAsync("Administrator").Result))
            {
                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Administrator"
                }).Wait();

                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Customer"
                }).Wait();

                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Tech"
                }).Wait();

                if (userManager.FindByNameAsync("Administrator").Result == null)
                {
                    var adminUser = new AppUser()
                    {
                        UserName = "Admin",
                        Email = "admin@mail.bg",
                        fName = "Admicho",
                        lName = "Adminchev"
                    };
                    IdentityResult adminCreated = userManager.CreateAsync(adminUser, "admin123").Result;
                    if (adminCreated.Succeeded)
                    {
                        userManager.AddToRoleAsync(adminUser, "Administrator").Wait();
                    }
                }
                if (userManager.FindByNameAsync("Customer").Result == null)
                {
                    var adminUser = new AppUser()
                    {
                        UserName = "Customer",
                        Email = "customer@mail.bg",
                        fName = "Customercho",
                        lName = "Customerchev"
                    };
                    IdentityResult adminCreated = userManager.CreateAsync(adminUser, "customer123").Result;
                    if (adminCreated.Succeeded)
                    {
                        userManager.AddToRoleAsync(adminUser, "Customer").Wait();
                    }
                }
                if (userManager.FindByNameAsync("Tech").Result == null)
                {
                    var adminUser = new AppUser()
                    {
                        UserName = "Tech",
                        Email = "Techer@mail.bg",
                        fName = "Techercho",
                        lName = "Techerchev"
                    };
                    IdentityResult adminCreated = userManager.CreateAsync(adminUser, "tech123").Result;
                    if (adminCreated.Succeeded)
                    {
                        userManager.AddToRoleAsync(adminUser, "Tech").Wait();
                    }
                }
            }
        }
    }
}