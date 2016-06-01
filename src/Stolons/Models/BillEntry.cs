
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

        [Display(Name = "Fiche produit")]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Display(Name = "Quantité")]
        public int Quantity { get; set; }

	[NotMapped]
	public string QuantityString
	{
	    get
	    {
		if (Product.Type == Product.SellType.Piece)
		{
		    if (Quantity == 1)
		    {
			return Quantity + " pièce";
		    }
		    else
		    {
			return Quantity + " pièces";
		    }
		}
		else
		{
		    float qty = (Quantity * Product.QuantityStep);
		    if (Product.ProductUnit == Product.Unit.Kg)
		    {
			string unit = " g";
			if (qty >= 1000)
			{
			    qty /= 1000;
			    unit = " Kg";
			}
			return qty + unit;
		    }
		    else
		    {
			string unit = " mL";
			if (qty >= 1000)
			{
			    qty /= 1000;
			    unit = " L";
			}
			return qty + unit;
		    }
		}
	    }
	}

	[NotMapped]
	public float Price
	{
	    get
	    {
		return Quantity * Product.UnitPrice;
	    }
	}

        public BillEntry Clone()
        {
            BillEntry clonedBillEntry = new BillEntry();
            clonedBillEntry.ProductId = this.ProductId;
            clonedBillEntry.Product = this.Product;
            clonedBillEntry.Quantity = this.Quantity;
            return clonedBillEntry;
        }
    }
}
