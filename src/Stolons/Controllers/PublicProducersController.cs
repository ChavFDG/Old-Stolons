using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using Microsoft.AspNet.Authorization;
using Newtonsoft.Json;

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

	[AllowAnonymous]
	[HttpGet, ActionName("Producers"), Route("api/producers")]
	public string JsonProducts() {
	    var producers = _context.Producers.ToList();

	    return JsonConvert.SerializeObject(producers, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
	}
    }
}
