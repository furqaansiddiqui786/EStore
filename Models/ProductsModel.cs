using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlipShop_OnlineShopping.Models
{
    public class ProductsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string About { get; set; }

        public byte[] ProductPhoto { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoriesModel CategoriesModel { get; set; }

        [Required]
        public double Price { get; set; }
    }
}
