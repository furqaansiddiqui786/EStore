using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FlipShop_OnlineShopping.Models;
using FlipShop_OnlineShopping.Data;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using FlipShop_OnlineShopping.Areas.Admin.Controllers;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FlipShop_OnlineShopping.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
  
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.ProductsModel.Include(s => s.CategoriesModel).ToListAsync();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", cnt);
            }


            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string productsearch)
        {
            ViewData["GetProducts"] = productsearch;
            var query = from x in _db.ProductsModel select x;

            if (!String.IsNullOrEmpty(productsearch))
            {
                query = query.Where(x => x.Name.Contains(productsearch));
            }
            return View(await query.AsNoTracking().ToListAsync());
        }
        


        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var menuitemfromdb = await _db.ProductsModel.Include(s=> s.CategoriesModel).Where(s => s.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartobj = new ShoppingCart()
            {
                Productitem = menuitemfromdb,
                ProductId = menuitemfromdb.Id
            };

            return View(cartobj);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cartobject)
        {
            cartobject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartobject.ApplicationUserId = claim.Value;

                ShoppingCart cartfromdb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == cartobject.ApplicationUserId && c.ProductId == cartobject.ProductId).FirstOrDefaultAsync();

                if(cartfromdb == null)
                {
                    _db.ShoppingCart.AddAsync(cartobject);
                }
                else
                {
                    cartfromdb.Count = cartfromdb.Count + cartobject.Count;
                }

                await _db.SaveChangesAsync();

                var Count = _db.ShoppingCart.Where(c => c.ApplicationUserId == cartobject.ApplicationUserId).ToList().Count();

                HttpContext.Session.SetInt32("ssCartCount", Count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productfromdb = await _db.ProductsModel.Include(s => s.CategoriesModel).Where(m=>m.Id == cartobject.ProductId).FirstOrDefaultAsync();

                ShoppingCart CartObj = new ShoppingCart()
                {
                    Productitem = productfromdb,
                    ProductId = productfromdb.Id
                };

                return View(CartObj);
            }
        }


        public IActionResult About()
        {
            return View();
        }
    }
}
