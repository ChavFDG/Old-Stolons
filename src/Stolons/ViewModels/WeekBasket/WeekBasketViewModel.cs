using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.ViewModels.WeekBasket
{
    public class WeekBasketViewModel
    {
        public Consumer Consumer { get; set; }

        public List<Product> Products { get; set; }

        public List<ProductType> ProductTypes { get; set;}

        public Models.WeekBasket WeekBasket { get; set; }

        public WeekBasketViewModel() 
        {
        }

        public WeekBasketViewModel(Consumer consumer, Models.WeekBasket weekBasket, ApplicationDbContext context)
        {
            WeekBasket = weekBasket;
            Consumer = consumer;
            Products = context.Products.Include(x=>x.Producer).Where(x => x.State == Product.ProductState.Enabled).ToList();
            ProductTypes = context.ProductTypes.Include(x => x.ProductFamilly).ToList();
        }
    }
}
