using System.Linq;
using Microsoft.AspNet.Mvc;
using Stolons.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using Stolons.ViewModels.WeekBasket;

namespace Stolons.Controllers
{
    public class WeekBasketController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public WeekBasketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: WeekBasket/Index/id
        public async Task<IActionResult> Index(int? id)
        {
            Consumer consumer = _context.Consumers.Single(m => m.Id == id);
            if (consumer == null)
            {
                return HttpNotFound();
            }
            return View(new WeekBasketViewModel(consumer, _context));
        }
    }
}
