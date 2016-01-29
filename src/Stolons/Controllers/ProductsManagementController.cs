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
            var products = _context.Producs.Include(m => m.Labels).Include(m => m.Familly).Where(x => x.Producer.Email == appUser.Email).ToList();
            return View(products);
        }

        // GET: ProductsManagement/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Producs.Single(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // GET: ProductsManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductsManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile uploadFile1, IFormFile uploadFile2, IFormFile uploadFile3)
        {
            if (ModelState.IsValid)
            {
                //On s'occupe des images du produit
                string fileName = Configurations.DefaultFileName;
                foreach (IFormFile uploadFile in new List<IFormFile>() { uploadFile1, uploadFile2, uploadFile3 })
                {
                    if (uploadFile != null)
                    {
                        //Image uploading
                        string uploads = Path.Combine(_environment.WebRootPath, Configurations.UserProductsStockagePath);
                        fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                        await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName)); 
                        product.Pictures.Add(Path.Combine(Configurations.UserProductsStockagePath, fileName));
                    }
                }

                product.Id = Guid.NewGuid();
                _context.Producs.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: ProductsManagement/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Producs.Single(m => m.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: ProductsManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Update(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: ProductsManagement/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Product product = _context.Producs.Single(m => m.Id == id);
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
            Product product = _context.Producs.Single(m => m.Id == id);
            _context.Producs.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
    }
}
