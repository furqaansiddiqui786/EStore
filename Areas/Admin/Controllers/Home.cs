using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class Home : Controller
    {
        private readonly ApplicationDbContext _db;

        public Home(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
