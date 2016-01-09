using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class ProductFamilly
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Famille de produit")]
        public ProductType Type { get; set; }
        [Display(Name = "Nom")]
        public string Name { get; set; }
    }
}
