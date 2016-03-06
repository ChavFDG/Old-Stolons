
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class BillEntry
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public User User { get; set; }
        [Display(Name = "Fiche produit")]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Display(Name = "Quantité")]
        public int Quantity { get; set; }
        [Display(Name = "A valider, valider")]
        public bool Validate { get; set; }
    }
}
