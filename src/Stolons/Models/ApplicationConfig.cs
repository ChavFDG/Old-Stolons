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
        public DayOfWeek CommandDayStartDate { get; set; }
        [Display(Name = "Heure")]
        public int CommandHourStartDate { get; set; }
        [Display(Name = "Minute")]
        public int CommandMinuteStartDate { get; set; }
        
        [Display(Name = "Mode simulation")]
        public bool Simulation { get; set; }

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
