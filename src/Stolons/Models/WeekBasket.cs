using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class WeekBasket
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Consomateur")]
        public Consumer Consumer { get; set; }
        [Display(Name = "Produits")]
        public List<BillEntry> Products { get; set; }
    }
}
