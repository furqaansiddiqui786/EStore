using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlipShop_OnlineShopping.Models.ViewModel
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
