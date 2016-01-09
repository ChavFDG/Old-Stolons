using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class Consumer : User
    {
        [Display(Name = "Factures")]
        public List<Bill> Bills { get; set; }
        [Display(Name = "Panier de la semaine")]
        public WeekBasket weekBasket { get; set; }


    }
}
