using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using Microsoft.AspNet.Authorization;

namespace Stolons.Controllers
{
    public class PublicProductsController : Controller
    {
        private ApplicationDbContext _context;

        public PublicProductsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: PublicProducts
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }        
    }
}
