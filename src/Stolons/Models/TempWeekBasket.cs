using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class TempWeekBasket : IWeekBasket
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Consomateur")]
        public Consumer Consumer { get; set; }

        [Display(Name = "Produits")]
        public List<BillEntry> Products { get; set; }

	public bool Validated { get; set; }
    }
}
