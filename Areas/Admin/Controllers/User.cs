using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlipShop_OnlineShopping.Data;
using FlipShop_OnlineShopping.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace FlipShop_OnlineShopping.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminUser)]
    [Area("Admin")]
    public class User : Controller
    {

        private readonly ApplicationDbContext _db;

        public User(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var users = _db.ApplicationUser.Where(c => c.Id != claims.Value);

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string userSearch)
        {
            ViewData["GetUsers"] = userSearch;
            var query = from x in _db.ApplicationUser select x;

            if (!String.IsNullOrEmpty(userSearch))
            {
                query = query.Where(x => x.Name.Contains(userSearch));
            }
            return View(await query.AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> Lock(string id)
        {
            if(id == null) { return NotFound(); }
            var appuser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (appuser == null) { return NotFound(); }
            appuser.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null) { return NotFound(); }
            var appuser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (appuser == null) { return NotFound(); }
            appuser.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(string id)
        {
            if (id == null) 
            { 
                return NotFound(); 
            }

            var appuser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (appuser == null) 
            { 
                return NotFound(); 
            }

            _db.ApplicationUser.Remove(appuser);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
