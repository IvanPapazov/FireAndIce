using FireAndIce.Models;
using FireAndIce.Models.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FireAndIce.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: UsersController
        public ActionResult Index()
        {
            List<userViewModel> users = _userManager.Users
               .Select(item => new userViewModel()
               {
                   Id =item.Id,
                   fName = item.fName,
                   lName = item.lName,
                   UserName = item.UserName,
                   Email = item.Email,
                   Roles = string.Join(
                       ", ", _userManager.GetRolesAsync(item).Result
                   )
               })
               .ToList();
            return View(users);
        }

        // GET: UsersController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var userData = await _userManager.FindByIdAsync(id);
            var user = new userViewModel()
            {
                Id =userData.Id,
                UserName = userData.UserName,
                fName = userData.fName,
                lName = userData.lName,
                Email = userData.Email,
                Roles = string.Join(
                        ", ", _userManager.GetRolesAsync(userData).Result)
            };
            return View(user);
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            SelectList options = new SelectList(roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name));
            ViewBag.Create = options;
            return View(new createUserViewModel());
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] createUserViewModel model)
        {
            try
            {
                AppUser user = new AppUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    fName = model.fName,
                    lName = model.lName,
                    EmailConfirmed = true
                };

                IdentityResult result = _userManager.CreateAsync(user, model.Password).Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, model.Role.ToString()).Wait();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var userData = await _userManager.FindByIdAsync(id);
            var user = new createUserViewModel()
            {
                UserName = userData.UserName,
                fName = userData.fName,
                lName = userData.lName,
                Email = userData.Email,
                Role = string.Join(
                        ", ", _userManager.GetRolesAsync(userData).Result)

            };
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            SelectList options = new SelectList(roles, nameof(IdentityRole.Id), nameof(IdentityRole.Name));
            ViewBag.Create = options;
            return View(user);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, [FromForm] createUserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                user.UserName = model.UserName;
                user.fName = model.fName;
                user.lName = model.lName;
                user.Email = model.Email;
                var role = await _roleManager.FindByIdAsync(model.Role);
                if (model.Password != "")
                {
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, model.Password);
                }

                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var roles = new List<string>() { "Administrator", "Tech", "Customer" };
                    bool succeded = false;
                    foreach (var r in roles)
                    {
                        IdentityResult res2 = await _userManager.RemoveFromRoleAsync(user, r);
                        if (res2.Succeeded)
                        {
                            succeded = true;
                        }
                    }
                    if (succeded)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var userData = await _userManager.FindByIdAsync(id);
            var user = new userViewModel()
            {
                fName=userData.fName,
                lName=userData.lName,
                UserName = userData.UserName,
                Email = userData.Email,
                Roles = string.Join(
                        ", ", _userManager.GetRolesAsync(userData).Result)
            };
            return View(user);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, userViewModel model)
        {
            try
            {
                IdentityResult identityResult = await _userManager.DeleteAsync(
                    await _userManager.FindByIdAsync(id));
                if (identityResult.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
