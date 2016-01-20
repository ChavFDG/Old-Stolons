using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;

namespace Stolons.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
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
        public IActionResult Create(News news)
        {
            if (ModelState.IsValid)
            {
                news.Id = Guid.NewGuid();
                news.DateOfPublication = DateTime.Now;
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
        public IActionResult Edit(News news)
        {
            if (ModelState.IsValid)
            {
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
            _context.News.Remove(news);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
