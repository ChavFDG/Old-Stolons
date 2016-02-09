using Stolons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons
{
    public static class Configurations
    {
        public enum Role
        {
            [Display(Name = "Adhérent")]
            User = 1,
            [Display(Name = "Bénévole")]
            Volunteer = 2,
            [Display(Name = "Administrateur")]
            Administrator = 3
        }

        public enum UserType
        {
            [Display(Name = "Simple adhérent")]
            SimpleUser,
            [Display(Name = "Adhérent consomateur")]
            Consumer,
            [Display(Name = "Adhérent producteur")]
            Producer,
        }

        internal static IList<string> GetRoles()
        {
            return Enum.GetNames(typeof(Role));
        }

        internal static IList<string> GetUserTypes()
        {
            return Enum.GetNames(typeof(UserType));
        }
        
        public static string NewsImageStockagePath = Path.Combine("uploads", "images", "news");
        public static string UserAvatarStockagePath = Path.Combine("uploads", "images", "avatars");
        public static string UserProductsStockagePath = Path.Combine("uploads", "products");
        public static string DefaultFileName = "Default.png";

        public static string GetAlias(this User user)
        {
            if (user is Producer)
            {
                return (user as Producer).CompanyName;
            }
            else
            {
                return "Les Stolons de Privas";
            }
        }



        private static string _labelImagePath = Path.Combine("images", "labels");
        public static string GetImage(this Product.Label label)
        {
            return Path.Combine(_labelImagePath, label.ToString() + ".jpg");
        }
    }
}
