using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Stolons.Models
{
    public class Label
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Display(Name = "Image")] //Lien vers l'image du label
        public string Image { get; set; }
    }
}
