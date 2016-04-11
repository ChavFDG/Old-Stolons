using System.Linq;
using Microsoft.AspNet.Mvc;
using Stolons.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using Stolons.ViewModels.WeekBasket;
using Microsoft.Data.Entity;
using System.Security.Claims;
using System.Collections.Generic;

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
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x => x.Consumer).Include(x=>x.Products).FirstOrDefault(x => x.Consumer.Id == consumer.Id);
            ValidatedWeekBasket validatedWeekBasket = _context.ValidatedWeekBaskets.Include(x => x.Consumer).Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == consumer.Id);
            if (tempWeekBasket == null)
            {
                //Il n'a pas encore de panier de la semaine, on lui en crée un
                tempWeekBasket = new TempWeekBasket();
                tempWeekBasket.Consumer = consumer;
                tempWeekBasket.Products = new System.Collections.Generic.List<BillEntry>();
                _context.Add(tempWeekBasket);
                _context.SaveChanges();
            }
            return View(new WeekBasketViewModel(consumer,tempWeekBasket, validatedWeekBasket, _context));
        }

        [HttpPost, ActionName("AddToBasket")]
        public BillEntry AddToBasket(string weekBasketId, string productId)
        {
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x=>x.Consumer).Include(x=>x.Products).First(x => x.Id.ToString() == weekBasketId);
            BillEntry billEntry = new BillEntry();
            billEntry.Product = _context.Products.First(x => x.Id.ToString() == productId);
            billEntry.Quantity = 1;
            tempWeekBasket.Products.Add(billEntry);
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
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x=>x.Consumer).Include(x => x.Products).First(x => x.Id.ToString() == weekBasketId);
            BillEntry billEntry = tempWeekBasket.Products.First(x => x.ProductId.ToString() == productId);
            billEntry.Product = _context.Products.First(x => x.Id.ToString() == productId);
            billEntry.Quantity = billEntry.Quantity + quantity;
            if (billEntry.Quantity == 0)
            {
                //La quantité est à 0 on supprime le produit
                _context.Remove(billEntry);
            }            
            _context.SaveChanges();
            return billEntry;
        }

        [HttpPost, ActionName("ValidateBasket")]
        public IActionResult ValidateBasket(string basketId)
        {
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x => x.Products).Include(x=>x.Consumer).First(x => x.Id.ToString() == basketId);
            ValidatedWeekBasket validatedWeekBasket = _context.ValidatedWeekBaskets.Include(x => x.Consumer).Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == tempWeekBasket.Consumer.Id);
            if (validatedWeekBasket == null)
            {
                //First validation of the week
                validatedWeekBasket = new ValidatedWeekBasket();
                validatedWeekBasket.Products = new List<BillEntry>();
                validatedWeekBasket.Consumer = tempWeekBasket.Consumer;
                _context.Add(validatedWeekBasket);
            }
            List<BillEntry> unValidBillEntry = new List<BillEntry>();
            float totalPrice = 0;
            //LOCK to prevent multi insert at this momment
            foreach(BillEntry billEntry in tempWeekBasket.Products.ToList())
            {
                Product product = _context.Products.First(x => x.Id == billEntry.ProductId);
                BillEntry validatedBillEntry = validatedWeekBasket.Products.FirstOrDefault(x => x.ProductId == product.Id);
                int realQuantity = billEntry.Quantity;
                if (validatedBillEntry != null)
                    realQuantity = billEntry.Quantity - validatedBillEntry.Quantity;
                //There is enouth stock
                if (billEntry.Quantity <= product.RemainingStock)
                {
                    //On met à jour le panier valide
                    if(validatedBillEntry == null)
                    {
                        validatedWeekBasket.Products.Add(billEntry.Clone());
                    }
                    else
                    {
                        validatedBillEntry.Quantity = billEntry.Quantity;
                    }
                    product.RemainingStock = product.RemainingStock - realQuantity;
                    totalPrice += billEntry.Quantity * product.Price;
                }
                //Not enouth stock, we don't valid this product
                else
                {
                    unValidBillEntry.Add(billEntry);
                }
            }
            _context.SaveChanges();
            //END LOCK
            return View(new ValidationSummaryViewModel(validatedWeekBasket, unValidBillEntry) { Total = totalPrice });
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
