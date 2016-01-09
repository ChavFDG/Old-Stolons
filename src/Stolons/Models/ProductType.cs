using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class ProductType
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Nom")]
        public string Name { get; set; }
    }
}
