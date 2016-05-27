using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class ApplicationConfig
    {

        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Libelle de la structure (Stolons)")]
        public string StolonsLabel { get; set; }
        [Display(Name = "Adresse de la structure")]
        public string StolonsAddress { get; set; }
        [Display(Name = "Numéro de téléphone de la structure")]
        public string StolonsPhoneNumber { get; set; }
        
        [Display(Name = "Texte de la page \"qui somme nous\"")]
        public string StolonsAboutPageText { get; set; }
        //Mails
        [Display(Name = "Courriel d'envoie des mails")]
        public string MailAddress { get; set; }
        [Display(Name = "Mot de passe du courriel")]
        public string MailPassword{ get; set; }
        [Display(Name = "Smtp")]
        public string MailSmtp { get; set; }
        [Display(Name = "Port")]
        public int MailPort{ get; set; }


        //
        [Display(Name = "Message de récupération du panier (jour, lieu, plage horraire)")]
        public string OrderDeliveryMessage { get; set; }

        [Display(Name = "Jour")]
        public DayOfWeek PreparationDayStartDate{ get; set; }
        [Display(Name = "Heure")]
        public int PreparationHourStartDate { get; set; }
        [Display(Name = "Minute")]
        public int PreparationMinuteStartDate { get; set; }
        
        [Display(Name = "Jour")]
        public DayOfWeek StockUpdateDayStartDate { get; set; }
        [Display(Name = "Heure")]
        public int StockUpdateHourStartDate { get; set; }
        [Display(Name = "Minute")]
        public int StockUpdateMinuteStartDate { get; set; }
        
        [Display(Name = "Jour")]
        public DayOfWeek OrderDayStartDate { get; set; }
        [Display(Name = "Heure")]
        public int OrderHourStartDate { get; set; }
        [Display(Name = "Minute")]
        public int OrderMinuteStartDate { get; set; }
        
        [Display(Name = "Mode simulation")]
        public bool IsModeSimulated { get; set; }

        [Display(Name = "Choix du mode à simuler")]
        public Modes SimulationMode { get; set; }

        public enum Modes
        {
            [Display(Name = "Commandes")]
            Order = 0,
            [Display(Name = "Préparation des commandes")]
            Preparation = 1,
            [Display(Name = "Mise à jour des stocks")]
            StockUpdate = 2
        }

    }
}
