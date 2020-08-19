using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FlipShop_OnlineShopping.Data;
using FlipShop_OnlineShopping.Models;
using FlipShop_OnlineShopping.Models.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlipShop_OnlineShopping.utilities;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Authorization;

namespace FlipShop_OnlineShopping.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SiteManagers)]
    [Area("Admin")]
    public class Products : Controller
    {
        private readonly ApplicationDbContext _db;
      

        public Products(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _db.ProductsModel.Include(s => s.CategoriesModel).AsNoTracking().OrderBy(s => s.Name);
            var model = await PagingList.CreateAsync(query,5,page);
            return View(model);
        }
        

        //[HttpGet] for search 
        //public async Task<IActionResult> Index(string productsearch)
        //{
        //    ViewData["GetProducts"] = productsearch;
        //    var query = from x in _db.ProductsModel select x;

        //    if (!String.IsNullOrEmpty(productsearch))
        //    {
        //        query = query.Where(x => x.Name.Contains(productsearch));
        //    }
        //    return View(await query.AsNoTracking().ToListAsync());
        //}

        public async Task<IActionResult> AddProducts()
        {
            ProductAndCategoryViewModel PAC = new ProductAndCategoryViewModel()
            {
                CategoryList = await _db.categoriesModels.ToListAsync(),
                Products = new Models.ProductsModel()
            };
            return View(PAC);
        }



        [TempData]
        public string StatusMessage { get; set; }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProducts(ProductAndCategoryViewModel PAC)
        {
           
            var menuItemFromDb = await _db.ProductsModel.FindAsync(PAC.Products.Id);


            if (ModelState.IsValid)
            {
                var duplicate = _db.ProductsModel.Include(s => s.CategoriesModel).Where(s => s.Name == PAC.Products.Name && s.CategoriesModel.Id == PAC.Products.CategoryId);
                if (duplicate.Count() > 0)
                {
                    StatusMessage = "Error : Product Already exists under " + duplicate.First().CategoriesModel.Name + " Category, Please use Another Name";
                }
                else
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count > 0)
                    {
                        byte[] p1 = null;
                        using (var fs1 = files[0].OpenReadStream())
                        {
                            using (var ms1 = new MemoryStream())
                            {
                                fs1.CopyTo(ms1);
                                p1 = ms1.ToArray();
                            }
                        }
                        PAC.Products.ProductPhoto = p1;
                    }
                    
                    _db.ProductsModel.Add(PAC.Products);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ProductAndCategoryViewModel mv = new ProductAndCategoryViewModel()
            {
                CategoryList = await _db.categoriesModels.ToListAsync(),
                Products = new Models.ProductsModel(),
                StatusMessage = StatusMessage
            };
            return View(mv);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _db.ProductsModel.SingleOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            ProductAndCategoryViewModel PAC = new ProductAndCategoryViewModel()
            {
                CategoryList = await _db.categoriesModels.ToListAsync(),
                Products = products
            };
            return View(PAC);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductAndCategoryViewModel PAC)
        {
            if (ModelState.IsValid)
            {
                var product = await _db.ProductsModel.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    product.ProductPhoto = p1;
                }
                product.Name = PAC.Products.Name;
                product.About = PAC.Products.About;
                product.CategoryId = PAC.Products.CategoryId;
                product.Price = PAC.Products.Price;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            ProductAndCategoryViewModel mv = new ProductAndCategoryViewModel()
            {
                CategoryList = await _db.categoriesModels.ToListAsync(),
                Products = new Models.ProductsModel(),
                StatusMessage = StatusMessage
            };
            return View(mv);
        }

        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var products = await _db.ProductsModel.SingleOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            ProductAndCategoryViewModel PAC = new ProductAndCategoryViewModel()
            {
                CategoryList = await _db.categoriesModels.ToListAsync(),
                Products = products
            };
            return View(PAC);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _db.ProductsModel.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _db.ProductsModel.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

    
}
