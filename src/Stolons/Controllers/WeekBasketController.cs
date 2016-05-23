using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using System.Security.Claims;
using System.Collections.Generic;
using System.Text;
using MimeKit;
using System.IO;
using Newtonsoft.Json;
using Stolons.Models;
using Stolons.ViewModels.WeekBasket;

namespace Stolons.Controllers
{
    [Authorize]
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
                //Il n'a pas encore de panier de la semaine, on lui en cr√©e un
                tempWeekBasket = new TempWeekBasket();
                tempWeekBasket.Consumer = consumer;
                tempWeekBasket.Products = new System.Collections.Generic.List<BillEntry>();
                _context.Add(tempWeekBasket);
                _context.SaveChanges();
            }
            return View(new WeekBasketViewModel(consumer,tempWeekBasket, validatedWeekBasket, _context));
        }

	[AllowAnonymous]
	[HttpGet, ActionName("Products"), Route("api/products")]
	public string JsonProducts() {
	    var Products = _context.Products.Include(x=>x.Producer).Include(x=>x.Familly).Where(x => x.State == Product.ProductState.Enabled).ToList();
	    return JsonConvert.SerializeObject(Products, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
	}

	[AllowAnonymous]
	[HttpGet, ActionName("ProductTypes"), Route("api/productTypes")]
	public string JsonProductTypes() {
	    var ProductTypes = _context.ProductTypes.Include(x => x.ProductFamilly).ToList();
	    return JsonConvert.SerializeObject(ProductTypes, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
	}

	/**
	 * Get or creates the temp week basket and explicitely load associated data
	 */
	private void loadBasketProducts(IWeekBasket basket)
	{
	    if (basket != null)
	    {
		foreach (BillEntry entry in basket.Products)
		{
		    //Ugly hack, fucking stupid lazy (and eager) loading
		    entry.Product = _context.Products.First(x => x.Id == entry.ProductId);
		}
	    }
	}	

	[HttpGet, ActionName("TmpWeekBasket"), Route("api/tmpWeekBasket")]
	public async Task<string> JsonTmpWeekBasket() {
	    var appUser = await GetCurrentUserAsync();
            Consumer consumer = _context.Consumers.FirstOrDefault(x => x.Email == appUser.Email);
            if (consumer == null)
            {
                return null;
            }
	    TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == consumer.Id);
	    if (tempWeekBasket == null)
            {
                //Il n'a pas encore de panier de la semaine, on lui en creer un
                tempWeekBasket = new TempWeekBasket();
                tempWeekBasket.Consumer = consumer;
                tempWeekBasket.Products = new System.Collections.Generic.List<BillEntry>();
                _context.Add(tempWeekBasket);
                _context.SaveChanges();
	    }
	    else
	    {
		loadBasketProducts(tempWeekBasket);
	    }
	    return JsonConvert.SerializeObject(tempWeekBasket, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
	}

	[HttpGet, ActionName("ValidatedWeekBasket"), Route("api/validatedWeekBasket")]
	public async Task<string> JsonValidatedWeekBasket() {
	    var appUser = await GetCurrentUserAsync();
            Consumer consumer = _context.Consumers.FirstOrDefault(x => x.Email == appUser.Email);
            if (consumer == null)
            {
                return null;
            }
	    ValidatedWeekBasket validatedWeekBasket = _context.ValidatedWeekBaskets.Include(x => x.Consumer).Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == consumer.Id);
	    loadBasketProducts(validatedWeekBasket);
	    return JsonConvert.SerializeObject(validatedWeekBasket, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
	}

        [HttpPost, ActionName("AddToBasket"), Route("api/addToBasket")]
        public string AddToBasket(string weekBasketId, string productId)
        {
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x=>x.Consumer).Include(x => x.Products).First(x => x.Id.ToString() == weekBasketId);
	    loadBasketProducts(tempWeekBasket);
            BillEntry billEntry = new BillEntry();
            billEntry.Product = _context.Products.First(x => x.Id.ToString() == productId);
            billEntry.Quantity = 1;
            tempWeekBasket.Products.Add(billEntry);
	    tempWeekBasket.Validated = isBasketValidated(tempWeekBasket);
            _context.SaveChanges();
	    return JsonConvert.SerializeObject(tempWeekBasket, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
        }

        [HttpPost, ActionName("PlusProduct"), Route("api/incrementProduct")]
        public string PlusProduct(string weekBasketId, string productId)
        {
	    return JsonConvert.SerializeObject(AddProductQuantity(weekBasketId, productId, +1), Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
        }

        [HttpPost, ActionName("MinusProduct"), Route("api/decrementProduct")]
        public string MinusProduct(string weekBasketId, string productId)
        {
	    return JsonConvert.SerializeObject(AddProductQuantity(weekBasketId, productId, -1), Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
        }

        private TempWeekBasket AddProductQuantity(string weekBasketId, string productId, int quantity)
        {
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x=>x.Consumer).Include(x => x.Products).First(x => x.Id.ToString() == weekBasketId);
	    loadBasketProducts(tempWeekBasket);
            BillEntry billEntry = tempWeekBasket.Products.First(x => x.ProductId.ToString() == productId);
            billEntry.Product = _context.Products.First(x => x.Id.ToString() == productId);

            if (quantity > 0 && billEntry.Product.RemainingStock < billEntry.Quantity + quantity)
	    {
                return tempWeekBasket;
            }

            billEntry.Quantity = billEntry.Quantity + quantity;

            if (billEntry.Quantity <= 0)
            {
                //La quantit√© est 0 on supprime le produit
                _context.Remove(billEntry);
            }
	    tempWeekBasket.Validated = isBasketValidated(tempWeekBasket);
            _context.SaveChanges();
            return tempWeekBasket;
        }

	[HttpPost, ActionName("RemoveBillEntry"), Route("api/removeBillEntry")]
        public string RemoveBillEntry(string weekBasketId, string productId)
        {
	    TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x=>x.Consumer).Include(x => x.Products).First(x => x.Id.ToString() == weekBasketId);
	    loadBasketProducts(tempWeekBasket);
            BillEntry billEntry = tempWeekBasket.Products.First(x => x.ProductId.ToString() == productId);
	    _context.Remove(billEntry);
	    tempWeekBasket.Validated = isBasketValidated(tempWeekBasket);
	    _context.SaveChanges();
	    return JsonConvert.SerializeObject(tempWeekBasket, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
        }

        [HttpPost, ActionName("ResetBasket"), Route("api/resetBasket")]
        public string ResetBasket(string basketId)
        {
            TempWeekBasket tempWeekBasket = _context.TempsWeekBaskets.Include(x => x.Products).Include(x=>x.Consumer).First(x => x.Id.ToString() == basketId);
            ValidatedWeekBasket validatedWeekBasket = _context.ValidatedWeekBaskets.Include(x => x.Consumer).Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == tempWeekBasket.Consumer.Id);

            if (validatedWeekBasket == null)
	    {
		tempWeekBasket.Products = new List<BillEntry>();
	    }
	    else
	    {
		tempWeekBasket.Products = new List<BillEntry>();
		foreach(BillEntry billEntry in validatedWeekBasket.Products.ToList())
		{
		    tempWeekBasket.Products.Add(billEntry.Clone());
		}
	    }
	    tempWeekBasket.Validated = true;
	    _context.SaveChanges();
	    loadBasketProducts(tempWeekBasket);
	    return JsonConvert.SerializeObject(tempWeekBasket, Formatting.Indented, new JsonSerializerSettings() {
		    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
			});
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
                        //On met ‡ jour le panier valide
                        if (validatedBillEntry == null)
                        {
                            //C'est un nouveau panier, il n'existe pas donc on l'ajoute, sinon c'est que le produit a ÈtÈ supprimÈ !
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
                Services.AuthMessageSender.SendEmail(validatedWeekBasket.Consumer.Email, validatedWeekBasket.Consumer.Name, "Panier de la semaine annulÈ", base.RenderPartialViewToString("ValidateBasket", null));
                _context.Remove(tempWeekBasket);
                _context.Remove(validatedWeekBasket);
                _context.SaveChanges();
                return View();
            }
        }

	private bool isBasketValidated(TempWeekBasket tmpBasket)
	{
	    ValidatedWeekBasket validatedBasket = _context.ValidatedWeekBaskets.Include(x => x.Consumer).Include(x => x.Products).FirstOrDefault(x => x.Consumer.Id == tmpBasket.Consumer.Id);

	    if (validatedBasket == null)
	    {
		return false;
	    }
	    if (validatedBasket.Products.Count != tmpBasket.Products.Count)
	    {
		return false;
	    }
	    int nbChecked = tmpBasket.Products.Count;
	    foreach (BillEntry billEntry in tmpBasket.Products.ToList())
	    {
		foreach (BillEntry validatedEntry in validatedBasket.Products.ToList())
		{
		    if (billEntry.ProductId == validatedEntry.ProductId)
		    {
			if (billEntry.Quantity != validatedEntry.Quantity)
			{
			    return false;
			}
			else
			{
			    nbChecked--;
			}
		    }
		}
	    }
	    return nbChecked == 0;
	}

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
