using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;

namespace Stolons.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext _context;
        private IHostingEnvironment _environment;

        public NewsController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;    
        }

        // GET: News
        public IActionResult Index()
        {
            return View(_context.News.ToList());
        }

        // GET: News/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            News news = _context.News.Single(m => m.Id == id);
            if (news == null)
            {
                return HttpNotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                string fileName = Configurations.DefaultFileName;
                if (uploadFile != null)
                {
                    //Image uploading
                    string uploads = Path.Combine(_environment.WebRootPath, Configurations.NewsImageStockagePath);
                    fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                }
                //Setting value for creation
                news.Id = Guid.NewGuid();
                news.DateOfPublication = DateTime.Now;
                news.ImageLink = Path.Combine(Configurations.NewsImageStockagePath,fileName);
                //TODO Get logged in User and add it to the news
                //news.User = ???
                _context.News.Add(news);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            News news = _context.News.Single(m => m.Id == id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(News news,IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string uploads = Path.Combine(_environment.WebRootPath, Configurations.NewsImageStockagePath);
                    //Deleting old image
                    string oldImage = Path.Combine(uploads, news.ImageLink);
                    if (System.IO.File.Exists(oldImage) && news.ImageLink != Path.Combine(Configurations.NewsImageStockagePath,Configurations.DefaultFileName))
                        System.IO.File.Delete(Path.Combine(uploads, news.ImageLink));
                    //Image uploading
                    string fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                    //Setting new value, saving
                    news.ImageLink = Path.Combine(Configurations.NewsImageStockagePath, fileName);
                }
                _context.Update(news);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            News news = _context.News.Single(m => m.Id == id);
            if (news == null)
            {
                return HttpNotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            News news = _context.News.Single(m => m.Id == id);
            //Deleting image
            string uploads = Path.Combine(_environment.WebRootPath, Configurations.NewsImageStockagePath);
            string image = Path.Combine(uploads, news.ImageLink);
            if (System.IO.File.Exists(image) && news.ImageLink != Path.Combine(Configurations.NewsImageStockagePath, Configurations.DefaultFileName))
                System.IO.File.Delete(Path.Combine(uploads, news.ImageLink));
            _context.News.Remove(news);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
