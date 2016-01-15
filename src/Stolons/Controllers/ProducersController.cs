using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;

namespace Stolons.Controllers
{
    public class ProducersController : Controller
    {
        private ApplicationDbContext _context;

        public ProducersController(ApplicationDbContext context)
        {
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
        public IActionResult Create(Producer producer)
        {
            if (ModelState.IsValid)
            {
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
        public IActionResult Edit(Producer producer)
        {
            if (ModelState.IsValid)
            {
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
            _context.Producers.Remove(producer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
