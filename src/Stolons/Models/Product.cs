using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Producteur")]
        public Producer Producer { get; set; }
        [Display(Name = "Famille de produit")]
        public ProductFamilly Familly { get; set; }
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Label")]
        public List<Label> Labels { get; set; }


        private IList<string> _Pictures;
        [Display(Name = "Photos")]
        [NotMapped]
        public virtual IList<string> Pictures
        {
            get
            {

                return _Pictures;

            }
            set
            {
                _Pictures = value;
            }
        }

        public string PicturesSerialized
        {
            get
            {
                return Tools.SerializeListToString(_Pictures);
            }
            set
            {
                _Pictures = Tools.SerializeStringToList(value);
            }
        }

        [Display(Name = "DLC")]
        public DateTime DLC { get; set; }

        [Display(Name = "Type de vente")]
        public SellType Type { get; set; }
        [Display(Name = "Prix")]
        public float Price { get; set; }
        [Display(Name = "Stock de la semaine")]
        public int WeekStock { get; set; }
        [Display(Name = "Stock restant")]
        public int RemainingStock { get; set; }
        [Display(Name = "Pallier de poids")]
        public int QuantityStep { get; set; }
        [Display(Name = "Quantité moyenne")]
        public int AverageQuantity { get; set; }
        [Display(Name = "Unité de mesure")]
        public Unit ProductUnit { get; set; }
        [Display(Name = "Etat")]
        public ProductState State { get; set; }


        public enum SellType
        {
            Weigh = 0,
            Piece = 2,
            Wrapped = 3
        }

        public enum Unit
        {
            Gr = 0,
            Ml = 1
        }

        public enum ProductState
        {
            Disabled = 0,
            Enabled = 1,
            Stock = 2
        }
    }
}
