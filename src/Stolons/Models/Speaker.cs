using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class Speaker
    {
        [Key]
        public Guid SpeakerId { get; set; }
        //Essayé de faire fonctionner ça dans les vue de manière passif
        [Display(Name = "Le nom du speaker")]
        [Required]
        public string Name { get; set; }
        public string Bio { get; set; }
    }
}
