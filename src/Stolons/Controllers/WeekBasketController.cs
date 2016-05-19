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
using System.Text;
using MimeKit;
using System.IO;

namespace Stolons.Controllers
{
    public class WeekBasketController : BaseController
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
                //Il n'a pas encore de panier de la semaine, on lui en crï¿½e un
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

            if (quantity > 0 && billEntry.Product.RemainingStock < billEntry.Quantity + quantity) {
                return billEntry;
            }

            billEntry.Quantity = billEntry.Quantity + quantity;

            if (billEntry.Quantity == 0)
            {
                //La quantitï¿½ est ï¿½ 0 on supprime le produit
                _context.Remove(billEntry);
            }            
            _context.SaveChanges();
            return billEntry;
        }

        [HttpPost, ActionName("ValidateBasket")]
        public async Task<IActionResult> ValidateBasket(string basketId)
        {
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x => x.Products).Include(x=>x.Consumer).First(x => x.Id.ToString() == basketId);
            tempWeekBasket.RetrieveProducts(_context);
            ValidatedWeekBasket validatedWeekBasket = _context.ValidatedWeekBaskets.Include(x => x.Consumer).Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == tempWeekBasket.Consumer.Id);
            bool newBasket = false;
            if (validatedWeekBasket == null)
            {
                newBasket = true;
                //First validation of the week
                validatedWeekBasket = new ValidatedWeekBasket();
                validatedWeekBasket.Products = new List<BillEntry>();
                validatedWeekBasket.Consumer = tempWeekBasket.Consumer;
                _context.Add(validatedWeekBasket);
            }
            else
            {
                validatedWeekBasket.RetrieveProducts(_context);
            }
            List<BillEntry> unValidBillEntry = new List<BillEntry>();
            float totalPrice = 0;
            //LOCK to prevent multi insert at this momment
            if(tempWeekBasket.Products.Any())
            {
                foreach (BillEntry billEntry in tempWeekBasket.Products.ToList())
                {
                    BillEntry validatedBillEntry = validatedWeekBasket.Products.FirstOrDefault(x => x.ProductId == billEntry.ProductId);
                    int realQuantity = billEntry.Quantity;
                    int alreadyTakenQuantity = 0;
                    if (validatedBillEntry != null)
                    {
                        realQuantity = billEntry.Quantity - validatedBillEntry.Quantity;
                        alreadyTakenQuantity = validatedBillEntry.Quantity;
                    }
                    //There is enouth stock
                    if (billEntry.Quantity <= billEntry.Product.RemainingStock + alreadyTakenQuantity)
                    {
                        //On met à jour le panier valide
                        if (validatedBillEntry == null)
                        {
                            //C'est un nouveau panier, il n'existe pas donc on l'ajoute, sinon c'est que le produit a été supprimé !
                            if (newBasket)
                                validatedWeekBasket.Products.Add(billEntry.Clone());
                        }
                        else
                        {
                            validatedBillEntry.Quantity = billEntry.Quantity;
                        }
                        billEntry.Product.RemainingStock = billEntry.Product.RemainingStock - realQuantity;
                        totalPrice += billEntry.Quantity * billEntry.Product.Price;
                    }
                    //Not enouth stock, we don't valid this product
                    else
                    {
                        unValidBillEntry.Add(billEntry);
                    }
                }
                _context.SaveChanges();
                //END LOCK
                //Send email to user
                string subject;
                if (unValidBillEntry.Count == 0)
                {
                    subject = "Validation total de votre panier de la semaine";

                }
                else
                {
                    subject = "Validation partielle de votre panier de la semaine";
                }
                ValidationSummaryViewModel validationSummaryViewModel = new ValidationSummaryViewModel(validatedWeekBasket, unValidBillEntry) { Total = totalPrice };
                Services.AuthMessageSender.SendEmail(validatedWeekBasket.Consumer.Email, validatedWeekBasket.Consumer.Name, subject, base.RenderPartialViewToString("ValidateBasket", validationSummaryViewModel));
                //Return view
                return View(validationSummaryViewModel);
            }
            else
            {
                //Il ne commande rien du tout
                //On lui signale
                Services.AuthMessageSender.SendEmail(validatedWeekBasket.Consumer.Email, validatedWeekBasket.Consumer.Name, "Panier de la semaine annule", base.RenderPartialViewToString("ValidateBasket", null));
                _context.Remove(tempWeekBasket);
                _context.Remove(validatedWeekBasket);
                _context.SaveChanges();
                return View();
            }
        }


        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
