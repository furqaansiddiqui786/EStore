using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlipShop_OnlineShopping.Data;
using FlipShop_OnlineShopping.Models;
using FlipShop_OnlineShopping.utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlipShop_OnlineShopping.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SiteManagers)]
    [Area("Admin")]
    public class Categories : Controller
    {
        private readonly ApplicationDbContext _db;

        public Categories(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var cats = await _db.categoriesModels.ToListAsync();
            return View(cats);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoriesModel catmd)
        {
            if (ModelState.IsValid)
            {
                _db.categoriesModels.Add(catmd);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catmd);
        }

        //edit 
        public async Task<IActionResult>Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var cat = await _db.categoriesModels.FindAsync(id);
            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(CategoriesModel cat)
        {
            if(cat.Id == null)
            {
                return NotFound();
            }
            var catfromdb = await _db.categoriesModels.Where(c => c.Id == cat.Id).FirstOrDefaultAsync();
            if(catfromdb == null)
            {
                return NotFound();
            }
            catfromdb.Name = cat.Name;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //edit
        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cat = await _db.categoriesModels.FindAsync(id);
            return View(cat);
        }

        //delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var cat = await _db.categoriesModels.FindAsync(id);
            if(cat == null)
            {
                return NotFound();
            }
            _db.categoriesModels.Remove(cat);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
