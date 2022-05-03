using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FireAndIce.Data;
using FireAndIce.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace FireAndIce.Controllers
{
    public class QueriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<AppUser> _userManager;
        public QueriesController(ApplicationDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Assign(int? id)
        {
            List<AppUser> usersRoles = (await _userManager.GetUsersInRoleAsync("Tech")).ToList();
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query.FindAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            SelectList options = new SelectList(usersRoles, nameof(AppUser.Id), nameof(AppUser.UserName));
            ViewBag.Techs = options;
            return View(query);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Assign(int id, [Bind("Id,Title,Discription,Address,Immage,Status,DateVisit,TechId,OwnerId")] Query query)
        {
            if (id != query.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(query.TechId);

            if (ModelState.IsValid)
            {
                try
                {
                    if (query.DateVisit.Value.Day >= DateTime.Now.Day)
                    {
                        query.Tech = user;
                        _context.Update(query);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("The data must be after this day! Choose correct one!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                }
                return RedirectToAction(nameof(Index));
            }
            return View(query);
        }

        public async Task<IActionResult> TodayApplications()
        {
            return View(await _context.Query.Where(q => q.DateVisit.Value.Day == DateTime.Now.Day).ToListAsync());
        }

        // GET: Queries
        public async Task<IActionResult> Index(string status, string customer, string tech)
        {
            var user = await _userManager.GetUserAsync(User);
            List<AppUser> techs = (await _userManager.GetUsersInRoleAsync("Tech")).ToList();
            List<AppUser> customers = (await _userManager.GetUsersInRoleAsync("Customer")).ToList();
            SelectList options1 = new SelectList(techs, nameof(AppUser.Id), nameof(AppUser.UserName));
            ViewBag.Techs = options1;
            SelectList options2 = new SelectList(customers, nameof(AppUser.Id), nameof(AppUser.UserName));
            ViewBag.Customers = options2;

            AppUser customerObj = null;
            if (customer != null)
            {
                customerObj = await _userManager.FindByIdAsync(customer);
                return View(await _context.Query.Where(q => q.OwnerId == customerObj.Id).ToListAsync());
            }
            AppUser techObj = null;
            if (tech != null)
            {
                techObj = await _userManager.FindByIdAsync(tech);
                return View(await _context.Query.Where(q => q.TechId == techObj.Id).ToListAsync());
            }
            if (User.IsInRole("Customer"))
            {
                if (status == Status.Waiting.ToString())
                {
                    return View(await _context.Query.Where(q => q.Owner.Id == user.Id && q.Status == Status.Waiting).ToListAsync());
                }
                else if (status == Status.Completed.ToString())
                {
                    return View(await _context.Query.Where(q => q.Owner.Id == user.Id && q.Status == Status.Completed).ToListAsync());
                }
                else if (status == Status.ExpectingAVisit.ToString())
                {
                    return View(await _context.Query.Where(q => q.Owner.Id == user.Id && q.Status == Status.ExpectingAVisit).ToListAsync());
                }
                else if (status == Status.InProgress.ToString())
                {
                    return View(await _context.Query.Where(q => q.Owner.Id == user.Id && q.Status == Status.InProgress).ToListAsync());
                }
                else
                {
                    return View(await _context.Query.Where(q => q.Owner.Id == user.Id).ToListAsync());
                }
            }
            else if (User.IsInRole("Tech"))
            {
                return View(await _context.Query.Where(q => q.Tech.Id == user.Id).ToListAsync());
            }
            else
            {
                if (status == Status.Waiting.ToString())
                {
                    return View(await _context.Query.Where(q => q.Status == Status.Waiting).ToListAsync());
                }
                else if (status == Status.Completed.ToString())
                {
                    return View(await _context.Query.Where(q =>q.Status == Status.Completed).ToListAsync());
                }
                else if (status == Status.ExpectingAVisit.ToString())
                {
                    return View(await _context.Query.Where(q => q.Status == Status.ExpectingAVisit).ToListAsync());
                }
                else if (status == Status.InProgress.ToString())
                {
                    return View(await _context.Query.Where(q => q.Status == Status.InProgress).ToListAsync());
                }
                else
                {
                    return View(await _context.Query.ToListAsync());
                }
            }
        }
        // GET: Queries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query
                .FirstOrDefaultAsync(m => m.Id == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // GET: Queries/Create
        [Authorize(Roles = "Administrator, Customer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Queries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Customer")]
        public async Task<IActionResult> Create([Bind("Id,Title,Discription,Address,Immage,Status,DateVisit,TechId,OwnerId")] Query query, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                string filePath = "";
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    if (formFile.Length > 0)
                    {
                        // this line is needed for the proper creation of the file int wwwroot/images
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        filePath = Path.Combine(uploadsFolder, formFile.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        query.Immage = formFile.FileName;
                    }
                }
                var user = await _userManager.GetUserAsync(User);
                query.Owner = user;
                query.Status = Status.Waiting;
                query.DateVisit = null;
                _context.Add(query);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(query);
        }

        // GET: Queries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query.FindAsync(id);
            if (query == null)
            {
                return NotFound();
            }
            return View(query);
        }

        // POST: Queries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Discription,Address,Immage,Status,DateVisit")] Query query, List<IFormFile> files)
        {
            if (id != query.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string filePath = "";
                    if (files.Count > 0)
                    {
                        var formFile = files[0];
                        if (formFile.Length > 0)
                        {
                            // this line is needed for the proper creation of the file int wwwroot/images
                            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                            filePath = Path.Combine(uploadsFolder, formFile.FileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            query.Immage = formFile.FileName;
                        }
                    }
                    _context.Update(query);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QueryExists(query.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(query);
        }

        // GET: Queries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query
                .FirstOrDefaultAsync(m => m.Id == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // POST: Queries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = await _context.Query.FindAsync(id);
            _context.Query.Remove(query);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QueryExists(int id)
        {
            return _context.Query.Any(e => e.Id == id);
        }
    }
}
