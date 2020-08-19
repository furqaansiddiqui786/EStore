using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlipShop_OnlineShopping.Models.ViewModel
{
    public class OrderdetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
