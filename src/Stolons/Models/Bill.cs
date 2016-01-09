using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class Bill
    {
        [Key]
        [Display(Name = "Numéro de facture")] //NumAdherant_Annee_Semaine
        public string BillNumber { get; set; }
        [Display(Name = "Adhérant")]
        public User User { get; set; }
        [Display(Name = "Etat")]
        public BillState State { get; set; }

        public enum BillState
        {
            Pending = 0,
            Validated = 1
        }
    }
}
