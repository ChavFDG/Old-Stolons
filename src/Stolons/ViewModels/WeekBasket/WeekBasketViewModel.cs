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
        
        public List<ProductType> ProductTypes { get; set;}

        public WeekBasketViewModel() 
        {
        }

        public WeekBasketViewModel(Consumer consumer, ApplicationDbContext context)
        {
            Consumer = consumer;
            ProductTypes = context.ProductTypes.Include(x => x.ProductFamilly).ToList();
        }
    }
}
