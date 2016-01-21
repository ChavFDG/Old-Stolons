using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using Microsoft.AspNet.Hosting;
using System.IO;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Stolons.Controllers
{
    public class ConsumersController : Controller
    {
        private ApplicationDbContext _context;
        private IHostingEnvironment _environment;
        private string _userStockagePath = Path.Combine("uploads", "images", "avatars");
        private string _defaultFileName = "Default.png";

        public ConsumersController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;    
        }

        // GET: Consumers
        public IActionResult Index()
        {
            return View(_context.Consumers.ToList());
        }

        // GET: Consumers/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Consumer consumer = _context.Consumers.Single(m => m.Id == id);
            if (consumer == null)
            {
                return HttpNotFound();
            }

            return View(consumer);
        }

        // GET: Consumers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Consumers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Consumer consumer, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                string fileName = _defaultFileName;
                if (uploadFile != null)
                {
                    //Image uploading
                    string uploads = Path.Combine(_environment.WebRootPath, _userStockagePath);
                    fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                }
                //Setting value for creation
                consumer.Avatar = Path.Combine(_userStockagePath, fileName);
                consumer.RegistrationDate = DateTime.Now;
                _context.Consumers.Add(consumer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consumer);
        }

        // GET: Consumers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Consumer consumer = _context.Consumers.Single(m => m.Id == id);
            if (consumer == null)
            {
                return HttpNotFound();
            }
            return View(consumer);
        }

        // POST: Consumers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Consumer consumer, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string uploads = Path.Combine(_environment.WebRootPath, _userStockagePath);
                    //Deleting old image
                    string oldImage = Path.Combine(uploads, consumer.Avatar);
                    if (System.IO.File.Exists(oldImage) && consumer.Avatar != Path.Combine(_userStockagePath, _defaultFileName))
                        System.IO.File.Delete(Path.Combine(uploads, consumer.Avatar));
                    //Image uploading
                    string fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                    //Setting new value, saving
                    consumer.Avatar = Path.Combine(_userStockagePath, fileName);
                }
                _context.Update(consumer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consumer);
        }

        // GET: Consumers/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Consumer consumer = _context.Consumers.Single(m => m.Id == id);
            if (consumer == null)
            {
                return HttpNotFound();
            }

            return View(consumer);
        }

        // POST: Consumers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Consumer consumer = _context.Consumers.Single(m => m.Id == id);
            //Deleting image
            string uploads = Path.Combine(_environment.WebRootPath, _userStockagePath);
            string image = Path.Combine(uploads, consumer.Avatar);
            if (System.IO.File.Exists(image) && consumer.Avatar != Path.Combine(_userStockagePath, _defaultFileName))
                System.IO.File.Delete(Path.Combine(uploads, consumer.Avatar));
            _context.Consumers.Remove(consumer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
