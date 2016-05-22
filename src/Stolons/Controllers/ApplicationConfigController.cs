using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using Microsoft.AspNet.Authorization;
using static Stolons.Configurations;

namespace Stolons.Controllers
{
    public class ApplicationConfigController : Controller
    {
        private ApplicationDbContext _context;

        public ApplicationConfigController(ApplicationDbContext context)
        {
            _context = context;    
        }

        [Authorize(Roles = Role_Administrator)]
        // GET: ApplicationConfig
        public IActionResult Index()
        {
            return View(_context.ApplicationConfig.First());
        }

        [Authorize(Roles = Role_Administrator)]
        // GET: ApplicationConfig/Edit/5
        public IActionResult Edit()
        {
            return View(_context.ApplicationConfig.First());
        }

        // POST: ApplicationConfig/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Role_Administrator)]
        public IActionResult Edit(ApplicationConfig applicationConfig)
        {
            if (ModelState.IsValid)
            {
                Configurations.ApplicationConfig = applicationConfig;
                _context.Update(applicationConfig);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(applicationConfig);
        }
    }
}
