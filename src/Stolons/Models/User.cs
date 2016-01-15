using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public abstract class User
    {
        [Key]
        [Display(Name = "Identifiant de l'utilisateur")]
        public int Id { get; set; }
        [Display(Name = "Nom")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Prénom")]
        [Required]
        public string Surname { get; set; }
        [Display(Name = "Avatar")]
        public string Avatar { get; set; } //Lien vers l'image sur le serveur
        [Display(Name = "Adresse")] 
        public string Address { get; set; }
        [Display(Name = "Code postal")]
        public string PostCode { get; set; }
        [Display(Name = "Ville")]
        public string City { get; set; }
        [Display(Name = "Courriel")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Téléphone")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Display(Name = "Cotisation réglée")]
        public bool Cotisation { get; set; }
        [Display(Name = "Actif / Inactif")]
        public bool Enable { get; set; }
        [Display(Name = "Date d'enregistrement")]
        public DateTime RegistrationDate { get; set; }
        //Droits utilisateurs qui définie les droits en plus du type d'utilisateur
        [Display(Name = "Droits utilisateurs")]
        public Role UserRole { get; set; }

        public enum Role
        {
            [Display(Name = "Adhérent")]
            SimpleUser = 0,
            [Display(Name = "Adhérent consomateur")]
            ConsumerUser = 1,
            [Display(Name = "Adhérent bénévole")]
            Volunteer = 2,
            [Display(Name = "Adhérent administrateur")]
            Administrator = 3
        }
    }

}
