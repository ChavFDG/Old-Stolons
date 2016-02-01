using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Stolons.Models;

namespace Stolons.ViewModels.Banner
{
    public class BannerViewModel
    {
        public User User { get; set; }

        public BannerViewModel(User user)
        {
            User = user;
        }
    }
}
