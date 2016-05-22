using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using Microsoft.AspNet.Authorization;

namespace Stolons.Controllers
{
    public class PublicProducersController : Controller
    {
        private ApplicationDbContext _context;

        public PublicProducersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: PublicProducers
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_context.Producers.ToList());
        }        
    }
}
