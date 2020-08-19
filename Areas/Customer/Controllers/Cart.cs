using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlipShop_OnlineShopping.Data;
using FlipShop_OnlineShopping.Models;
using FlipShop_OnlineShopping.Models.ViewModel;
using FlipShop_OnlineShopping.utilities;
using Stripe;

namespace FlipShop_OnlineShopping.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class Cart : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public Cart(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var list in detailCart.listCart)
            {
                list.Productitem = await _db.ProductsModel.FirstOrDefaultAsync(m => m.Id == list.ProductId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.Productitem.Price * list.Count);

                list.Productitem.About = SD.ConvertToRawHtml(list.Productitem.About);

                if (list.Productitem.About.Length > 100)
                {
                    list.Productitem.About = list.Productitem.About.Substring(0, 99) + "...";
                }
            }
            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;
            return View(detailCart);
        }

        public async Task<IActionResult> plus(int cartid)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartid);
            cart.Count += 1;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> minus(int cartid)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartid);
            if (cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("ssCartCount", cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> remove(int cartid)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartid);
            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32("ssCartCount", cnt);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser Appuser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var list in detailCart.listCart)
            {
                list.Productitem = await _db.ProductsModel.FirstOrDefaultAsync(m => m.Id == list.ProductId);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (list.Productitem.Price * list.Count);
            }
            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;
            detailCart.OrderHeader.PickupName = Appuser.UserName;
            detailCart.OrderHeader.PhoneNumber = Appuser.PhoneNumber;
            detailCart.OrderHeader.PickUpDate = DateTime.Now;

            return View(detailCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Checkout")]
        public async Task<IActionResult> CheckoutPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailCart.listCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claim.Value;
            detailCart.OrderHeader.Status = SD.PaymentStatusPending;


            List<OrderDetail> orderDetails = new List<OrderDetail>();
            _db.OrderHeader.Add(detailCart.OrderHeader);
            await _db.SaveChangesAsync();

            detailCart.OrderHeader.OrderTotalOriginal = 0;

            foreach (var item in detailCart.listCart)
            {
                item.Productitem = await _db.ProductsModel.FirstOrDefaultAsync(m => m.Id == item.ProductId);

                OrderDetail orderdetails = new OrderDetail
                {
                    ProductItemId = item.ProductId,
                    OrderId = detailCart.OrderHeader.Id,
                    Description = item.Productitem.About,
                    Name = item.Productitem.Name,
                    Price = item.Productitem.Price,
                    Count = item.Count
                };
                detailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderdetails.Price;
                _db.OrderDetail.Add(orderdetails);
            }

  
            _db.ShoppingCart.RemoveRange(detailCart.listCart);
            HttpContext.Session.SetInt32("ssCartCount", 0);
            await _db.SaveChangesAsync();


            var options = new ChargeCreateOptions
            {
                Amount = 10000,
                Currency = "usd",
                Source = stripeToken,
                Description = "Order Id : " + detailCart.OrderHeader.Id,
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                detailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "Succeeded")
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                detailCart.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Confirm", "Order");
        }
    }
}
