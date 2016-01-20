using Stolons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class News
    {
        [Key]
        public Guid Id { get; set; }

        public User User { get; set; }

        [Required]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        public string ImageLink { get; set; }

        [Display(Name = "Publié le ")]
        public DateTime DateOfPublication { get; set; }
    }
}
