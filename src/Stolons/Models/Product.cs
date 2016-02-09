using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
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
        [Required]
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
        [Required]
        public SellType Type { get; set; }
        [Display(Name = "Prix")]
        [Required]
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
            [Display(Name ="Au poids")]
            Weigh = 0,
            [Display(Name = "A la pièce")]
            Piece = 1,
            [Display(Name = "Emballé")]
            Wrapped = 2
        }

        public enum Unit
        {
            Gr = 0,
            Ml = 1
        }

        public enum ProductState
        {
            [Display(Name = "Indisponible")]
            Disabled = 0,
            [Display(Name = "Disponible")]
            Enabled = 1,
            [Display(Name = "Attente Stock")]
            Stock = 2
        }

        public enum Label
        {
            [Display(Name = "AB")]
            Ab = 0,
            [Display(Name = "DEMETER")]
            Demeter = 1
        }
    }
}
