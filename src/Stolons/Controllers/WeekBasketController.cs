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
                weekBasket.Products = new System.Collections.Generic.List<BillEntry>();
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
            return AddProductQuantity(weekBasketId, productId, +1);
        }

        [HttpPost, ActionName("MinusProduct")]
        public BillEntry MinusProduct(string weekBasketId, string productId)
        {

            return AddProductQuantity(weekBasketId, productId, -1);
        }

        private BillEntry AddProductQuantity(string weekBasketId, string productId, int quantity)
        {
            WeekBasket weekBasket = _context.WeekBaskets.Include(x => x.Products).First(x => x.Id.ToString() == weekBasketId);
            BillEntry billEntry = weekBasket.Products.First(x => x.ProductId.ToString() == productId);
            billEntry.Product = _context.Products.First(x => x.Id.ToString() == productId);
            billEntry.Quantity = billEntry.Quantity + quantity;
            if(billEntry.Quantity == 0)
            {
                //La quantité est à 0 on supprime le produit
                _context.Remove(billEntry);
                billEntry = null;
            }
            else
            {
                billEntry.Validate = false;
                var test = _context.Entry(_context.Products.First(x => x.Id.ToString() == productId)).State ;
            }
            
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
