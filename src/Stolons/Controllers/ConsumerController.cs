using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Stolons.Models;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Stolons.Controllers
{
    public class ConsumerController : Controller
    {
        ApplicationDbContext _db;

        public ConsumerController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Consumers);
        }

        
        #region Add

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                _db.Consumers.Add(consumer);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consumer);
        }

        #endregion Add

        #region Detail

        [HttpGet]
        public IActionResult Detail(Guid topicId)
        {
            Topic topic = _db.Topics.Include(s => s.Speaker).FirstOrDefault(t => t.TopicId == topicId);

            return View(topic);
        }

        #endregion Detail

        #region Edit


        [HttpGet]
        public IActionResult Edit(int consumerId)
        {
            Consumer consumer = _db.Consumers.FirstOrDefault(x => x.Id == consumerId);
            return View(consumer);
        }

        [HttpPost]
        public IActionResult Edit(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                _db.Consumers.Update(consumer);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit");
        }

        #endregion Edit

    }
}
