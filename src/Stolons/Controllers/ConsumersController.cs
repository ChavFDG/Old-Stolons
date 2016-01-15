using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Stolons.Models;
using System;

namespace Stolons.Controllers
{
    public class ConsumersController : Controller
    {
        private ApplicationDbContext _context;

        public ConsumersController(ApplicationDbContext context)
        {
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
        public IActionResult Create(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
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
        public IActionResult Edit(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
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
            _context.Consumers.Remove(consumer);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
