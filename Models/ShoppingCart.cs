using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace FlipShop_OnlineShopping.Models
{
    public class ShoppingCart
    {

        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        [ForeignKey("ProductId")]
        public virtual ProductsModel Productitem { get; set; }


        [Range(1, int.MaxValue, ErrorMessage ="Please Select the quantity")]
        public int Count { get; set; }
    }
}
