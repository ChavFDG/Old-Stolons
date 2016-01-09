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
    public class TopicController : Controller
    {
        ApplicationDbContext _db;

        public TopicController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var topics = _db.Topics.Include(s => s.Speaker);

            return View(topics);
        }

        
        #region Add

        [HttpGet]
        public IActionResult Add()
        {

            var speakers = GetAllSpeakers();
            ViewBag.Speakers = speakers;
            
            return View();
        }
        
        private IEnumerable<SelectListItem> GetAllSpeakers()
        {
            return _db.Speakers.ToList().Select(speaker => new SelectListItem
            {
                Text = speaker.Name,
                Value = speaker.SpeakerId.ToString(),
            });
        }

        [HttpPost]
        public IActionResult Add(Topic topic)
        {
            _db.Topics.Add(topic);
            _db.SaveChanges();

            return RedirectToAction("Index");
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
        public IActionResult Edit(Guid topicId)
        {
            var speakers = GetAllSpeakers();
            ViewBag.Speakers = speakers;
            Topic topic = _db.Topics.Include(s => s.Speaker).FirstOrDefault(t => t.TopicId == topicId);

            return View(topic);
        }

        [HttpPost]
        public IActionResult Edit(Topic topic)
        {
            _db.Topics.Update(topic);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        #endregion Edit

        #region Delete
        [HttpGet]
        public IActionResult Delete(Guid topicId)
        {
            Topic topic = _db.Topics.Include(s => s.Speaker).FirstOrDefault(t => t.TopicId == topicId);

            return View(topic);
        }

        public IActionResult DeleteConfirmed(Guid topicId)
        {
            Topic topic = _db.Topics.FirstOrDefault(t => t.TopicId == topicId);
            _db.Topics.Remove(topic);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        #endregion Delete
    }
}
