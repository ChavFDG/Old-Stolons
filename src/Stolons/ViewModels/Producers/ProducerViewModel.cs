using Stolons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.ViewModels.Producers
{
    public class ProducerViewModel
    {
        public ProducerViewModel()
        {

        }
        public ProducerViewModel(Producer producer, Configurations.Role userRole)
        {
            Producer = producer;
            UserRole = userRole;
            OriginalEmail = producer.Email;
        }

        public string OriginalEmail { get; set; }
        public Producer Producer { get; set; }

        [Display(Name = "Droit utilisateur ")]
        public Configurations.Role UserRole { get; set; }
    }
}
