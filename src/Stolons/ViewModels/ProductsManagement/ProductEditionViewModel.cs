using Microsoft.AspNet.Http;
using Stolons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.ViewModels.ProductsManagement
{
    public class ProductEditionViewModel
    {


        public string[] SelectedLabels { get; set; }

        public string FamillyName { get; set; }

        public Product Product { get; set; }

        public List<ProductType> ProductTypes { get; set; }

        public IFormFile UploadFile1 { get; set; }
        public IFormFile UploadFile2 { get; set; }
        public IFormFile UploadFile3 { get; set; }

        public ProductEditionViewModel()
        {
        }

        public ProductEditionViewModel(Product product, List<ProductType> types)
        {
            Product = product;
            ProductTypes = types;
            SelectedLabels = product.Labels.OfType<string>().ToArray();

        }
    }
}
