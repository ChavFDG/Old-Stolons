using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Stolons.Models;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Stolons.Controllers
{
    public class SpeakerController : Controller
    {
        ApplicationDbContext _db;

        public SpeakerController(ApplicationDbContext db)
        {
            _db = db;
        }


        // GET: /<controller>/

        public IActionResult Index()
        {
            var speakers = _db.Speakers;

            return View(speakers);
        }

        //TEST

        #region Add
        [HttpGet]
        public IActionResult Add()
        {
            //return View();
            return PartialView("View");
        }
        /*
        [HttpPost]
        public IActionResult Add(Speaker speaker)
        {
            _db.Speakers.Add(speaker);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        */
        [HttpPost]
        public ActionResult Add(Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                _db.Speakers.Add(speaker);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return PartialView("View",speaker);
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Speaker speaker)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Speakers.Add(speaker);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
            }
            return PartialAdd();
        }

*/

        #endregion Add

        #region Detail

        [HttpGet]
        public IActionResult Detail(Guid speakerId)
        {
            var id = RouteData.Values["speakerId"];
            Speaker speaker = _db.Speakers.FirstOrDefault(s => s.SpeakerId == speakerId);

            return View(speaker);
        }

        #endregion Detail

        #region Edit

        [HttpGet]
        public IActionResult Edit(Guid speakerId)
        {
            Speaker speaker = _db.Speakers.FirstOrDefault(s => s.SpeakerId == speakerId);
            return View(speaker);
        }

        [HttpPost]
        public IActionResult Edit(Speaker speaker)
        {
            _db.Speakers.Update(speaker);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        #endregion Edit

        #region Delete

        [HttpGet]
        public IActionResult Delete(Guid speakerId)
        {

            Speaker speaker = _db.Speakers.FirstOrDefault(s => s.SpeakerId == speakerId);

            return View(speaker);

        }

        public IActionResult DeleteConfirmed(Guid speakerId)
        {
            Speaker speaker = _db.Speakers.FirstOrDefault(s => s.SpeakerId == speakerId);
            _db.Speakers.Remove(speaker);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        #endregion Delete
    }
}
