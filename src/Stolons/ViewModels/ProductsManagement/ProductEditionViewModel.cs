using Stolons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.ViewModels.ProductsManagement
{
    public class ProductEditionViewModel
    {
        public Product Product { get; set; }

        public List<ProductType> ProductTypes { get; set; }

        public ProductEditionViewModel()
        {
        }

        public ProductEditionViewModel(Product product, List<ProductType> types)
        {
            Product = product;
            ProductTypes = types;
        }
    }
}
