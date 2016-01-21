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
    public class ProducersController : Controller
    {
        private ApplicationDbContext _context;
        private IHostingEnvironment _environment;
        private string _userStockagePath = Path.Combine("uploads", "images", "avatars");
        private string _defaultFileName = "Default.png";

        public ProducersController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        // GET: Producer
        public IActionResult Index()
        {
            return View(_context.Producers.ToList());
        }

        // GET: Producer/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Producer producer = _context.Producers.Single(m => m.Id == id);
            if (producer == null)
            {
                return HttpNotFound();
            }

            return View(producer);
        }

        // GET: Producer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producer producer, IFormFile uploadFile)
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
                producer.Avatar = Path.Combine(_userStockagePath, fileName);
                producer.RegistrationDate = DateTime.Now;
                _context.Producers.Add(producer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producer);
        }

        // GET: Producer/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Producer producer = _context.Producers.Single(m => m.Id == id);
            if (producer == null)
            {
                return HttpNotFound();
            }
            return View(producer);
        }

        // POST: Producer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producer producer, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string uploads = Path.Combine(_environment.WebRootPath, _userStockagePath);
                    //Deleting old image
                    string oldImage = Path.Combine(uploads, producer.Avatar);
                    if (System.IO.File.Exists(oldImage) && producer.Avatar != Path.Combine(_userStockagePath, _defaultFileName))
                        System.IO.File.Delete(Path.Combine(uploads, producer.Avatar));
                    //Image uploading
                    string fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                    //Setting new value, saving
                    producer.Avatar = Path.Combine(_userStockagePath, fileName);
                }
                _context.Update(producer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(producer);
        }

        // GET: Producer/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Producer producer = _context.Producers.Single(m => m.Id == id);
            if (producer == null)
            {
                return HttpNotFound();
            }

            return View(producer);
        }

        // POST: Producer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Producer producer = _context.Producers.Single(m => m.Id == id);
            //Deleting image
            string uploads = Path.Combine(_environment.WebRootPath, _userStockagePath);
            string image = Path.Combine(uploads, producer.Avatar);
            if (System.IO.File.Exists(image) && producer.Avatar != Path.Combine(_userStockagePath, _defaultFileName))
                System.IO.File.Delete(Path.Combine(uploads, producer.Avatar));
            _context.Producers.Remove(producer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
