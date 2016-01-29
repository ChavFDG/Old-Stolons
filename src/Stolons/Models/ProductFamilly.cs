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
        [Display(Name = "Image")] //Lien vers l'image du label
        public string Image { get; set; }

        public ProductFamilly(ProductType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}
