using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Stolons.ViewModels.Banner;
using Stolons.Models;

namespace Stolons.Controllers
{
    public class BannerViewComponent : ViewComponent
    {
        private ApplicationDbContext _dbContext;
        
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger _logger;

        public BannerViewComponent(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory) {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<HomeController>();
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser appUser = await _userManager.FindByIdAsync(HttpContext.User.GetUserId()); 
                User user = _dbContext.StolonsUsers.FirstOrDefault(x => x.Email.Equals(appUser.Email, StringComparison.CurrentCultureIgnoreCase));
                return View(new BannerViewModel(user));
            } else
            {
                return View();
            }
        }

    }
}
