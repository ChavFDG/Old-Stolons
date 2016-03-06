using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Hosting;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.Net.Http.Headers;
using Stolons.ViewModels.ProductsManagement;

namespace Stolons.Controllers
{
    public class ProductsManagementController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public ProductsManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;    
        }

        // GET: ProductsManagement
        public async Task<IActionResult> Index()
        {
            var appUser = await GetCurrentUserAsync();
            var products = _context.Products.Include(m => m.Familly).Include(m=>m.Familly.Type).Where(x => x.Producer.Email == appUser.Email).ToList();
            return View(products);
        }

        // GET: ProductsManagement/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Products.Single(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: ProductsManagement/Create
        public IActionResult Manage(Guid? id)
        {
            Product product = id == null ? new Product() : _context.Products.Include(x=>x.Familly).First(x => x.Id == id);
            return View(new ProductEditionViewModel(product, _context,id == null));

        }

        // POST: ProductsManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ProductEditionViewModel vmProduct)
        {
            if (ModelState.IsValid)
            {
                //Set Labels
                vmProduct.Product.SetLabels(vmProduct.SelectedLabels);
                //Set Product familly (si ça retourne null c'est que la famille selectionnée n'existe pas, alors on est dans la merde)
                vmProduct.Product.Familly = _context.ProductFamillys.FirstOrDefault(x => x.FamillyName == vmProduct.FamillyName);
                //Set Producer (si ça retourne null, c'est que c'est pas un producteur qui est logger, alors on est dans la merde)
                var appUser = await GetCurrentUserAsync();
                vmProduct.Product.Producer = _context.Producers.FirstOrDefault(x => x.Email == appUser.Email);
                //On s'occupe des images du produit
                int cpt = 0;
                foreach (IFormFile uploadFile in new List<IFormFile>() { vmProduct.UploadFile1, vmProduct.UploadFile2, vmProduct.UploadFile3 })
                {
                    if (uploadFile != null)
                    {
                        //Image uploading
                        string fileName = await Configurations.UploadFile(_environment, uploadFile, Configurations.ProductsStockagePath);
                        if(!vmProduct.IsNew && vmProduct.Product.Pictures.Count > cpt)
                        {
                            //Replace
                            vmProduct.Product.Pictures[cpt] = fileName;
                        }
                        else
                        {
                            //Add
                            vmProduct.Product.Pictures.Add(fileName);
                        }
                    }
                    cpt++;
                }
                if(vmProduct.IsNew)
                {
                    vmProduct.Product.Id = Guid.NewGuid();
                    _context.Products.Add(vmProduct.Product);
                }
                else
                {
                    _context.Products.Update(vmProduct.Product);
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            vmProduct.RefreshTypes(_context);
            return View(vmProduct);
        }
        
        // GET: ProductsManagement/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Products.Single(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // POST: ProductsManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            Product product = _context.Products.Single(m => m.Id == id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Enable(Guid? id)
        {
            _context.Products.First(x => x.Id == id).State = Product.ProductState.Enabled;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult Disable(Guid? id)
        {
            _context.Products.First(x => x.Id == id).State = Product.ProductState.Disabled;
            _context.SaveChanges();
            return RedirectToAction("Index");

        }


        [HttpPost, ActionName("ChangeStock")]
        public IActionResult ChangeStock(Guid id, int newStock)
        {
            _context.Products.First(x => x.Id == id).WeekStock = newStock;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
