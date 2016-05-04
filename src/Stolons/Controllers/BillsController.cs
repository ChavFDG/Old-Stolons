using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using System.Security.Claims;

namespace Stolons.Controllers
{
    public class BillsController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public BillsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var appUser = await GetCurrentUserAsync();
            var stolonsUser = _context.StolonsUsers.First(x => x.Email == appUser.Email);
            if(stolonsUser is Producer)
            {
                return View(_context.ProducerBills.Where(x=>x.Producer.Email == stolonsUser.Email).OrderBy(x=>x.EditionDate).ToList<IBill>());
            }
            else if (stolonsUser is Consumer)
            {
                return View(_context.ConsumerBills.Where(x => x.Consumer.Email == stolonsUser.Email).OrderBy(x => x.EditionDate).ToList<IBill>());
            }
            return View();//ERROR
        }

        // GET: Bills/Download/5
        public async Task<IActionResult> Download(string id)
        {
            var appUser = await GetCurrentUserAsync();
            var stolonsUser = _context.StolonsUsers.First(x => x.Email == appUser.Email);
            if (stolonsUser is Producer)
            {
                return View(_context.ProducerBills.Where(x => x.Producer.Email == stolonsUser.Email).OrderBy(x => x.EditionDate).ToList<IBill>());
            }
            else if (stolonsUser is Consumer)
            {
                return View(_context.ConsumerBills.Where(x => x.Consumer.Email == stolonsUser.Email).OrderBy(x => x.EditionDate).ToList<IBill>());
            }
            
            return View( );
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
