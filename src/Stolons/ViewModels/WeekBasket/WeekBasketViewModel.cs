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

        public List<ProductFamilly> ProductFamilies { get; set;}

        public List<ProductType> ProductTypes { get; set;}

        public WeekBasketViewModel() 
        {
        }

        public WeekBasketViewModel(Consumer consumer, List<ProductFamilly> families, List<ProductType> types)
        {
            Consumer = consumer;
            ProductFamilies = families;
            ProductTypes = types;
        }
    }
}
