using System.Linq;
using Microsoft.AspNet.Mvc;
using Stolons.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using Stolons.ViewModels.WeekBasket;
using Microsoft.Data.Entity;
using System.Security.Claims;

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
        public async Task<IActionResult> Index()
        {
            var appUser = await GetCurrentUserAsync();
            Consumer consumer = _context.Consumers.FirstOrDefault(x => x.Email == appUser.Email);
            if (consumer == null)
            {
                return HttpNotFound();
            }
            WeekBasket weekBasket = _context.WeekBaskets.Include(x => x.Consumer).Include(x=>x.Products).FirstOrDefault(x => x.Consumer.Id == consumer.Id);
            if(weekBasket == null)
            {
                //Il n'a pas encore de panier de la semaine, on lui en crée un
                weekBasket = new WeekBasket();
                weekBasket.Consumer = consumer;
                _context.Add(weekBasket);
                _context.SaveChanges();
            }
            return View(new WeekBasketViewModel(consumer,weekBasket, _context));
        }

        [HttpPost, ActionName("AddToBasket")]
        public BillEntry AddToBasket(string weekBasketId, string productId)
        {
            WeekBasket weekBasket = _context.WeekBaskets.Include(x=>x.Products).First(x => x.Id.ToString() == weekBasketId);
            BillEntry billEntry = new BillEntry();
            billEntry.Product = _context.Products.First(x => x.Id.ToString() == productId);
            billEntry.Quantity = 1;
            billEntry.Validate = false;
            weekBasket.Products.Add(billEntry);
            _context.SaveChanges();
            return billEntry;
        }

        [HttpPost, ActionName("PlusProduct")]
        public BillEntry PlusProduct(string weekBasketId, string productId)
        {
            return AddProductQauntity(weekBasketId, productId, +1);
        }

        [HttpPost, ActionName("MinusProduct")]
        public BillEntry MinusProduct(string weekBasketId, string productId)
        {
            return AddProductQauntity(weekBasketId, productId, -1);
        }

        private BillEntry AddProductQauntity(string weekBasketId, string productId, int quantity)
        {
            WeekBasket weekBasket = _context.WeekBaskets.Include(x => x.Products).First(x => x.Id.ToString() == weekBasketId);
            BillEntry billEntry = _context.BillEntrys.Include(x => x.Product).First(x => x.Product.Id.ToString() == productId);
            billEntry.Quantity = billEntry.Quantity + quantity;
            billEntry.Validate = false;
            _context.Update(billEntry);
            _context.SaveChanges();
            return billEntry;

        }

        [HttpPost, ActionName("ValidateBasket")]
        public void ValidateBasket(string weekBasketId)
        {
            //TODO
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
