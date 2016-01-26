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
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Stolons.ViewModels.Producers;

namespace Stolons.Controllers
{
    public class ProducersController : Controller
    {
        private ApplicationDbContext _context;
        private IHostingEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProducersController(ApplicationDbContext context, IHostingEnvironment environment,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _environment = environment;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Producer
        public IActionResult Index()
        {
            return View(_context.Producers.ToList());
        }

        // GET: Producer/Details/5
        public async Task<IActionResult> Details(int? id)
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
            ApplicationUser appUser = _context.Users.First(x => x.Email == producer.Email);
            IList<string> roles = await _userManager.GetRolesAsync(appUser);
            string role = roles.FirstOrDefault(x => Configurations.GetRoles().Contains(x));
            return View(new ProducerViewModel(producer, (Configurations.Role)Enum.Parse(typeof(Configurations.Role), role)));
        }

        // GET: Producer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProducerViewModel vmProducer, IFormFile uploadFile)
        {

            if (ModelState.IsValid)
            {
                #region Creating Producer
                string fileName = Configurations.DefaultFileName;
                if (uploadFile != null)
                {
                    //Image uploading
                    string uploads = Path.Combine(_environment.WebRootPath, Configurations.UserAvatarStockagePath);
                    fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                }
                //Setting value for creation
                vmProducer.Producer.Avatar = Path.Combine(Configurations.UserAvatarStockagePath, fileName);
                vmProducer.Producer.RegistrationDate = DateTime.Now;
                _context.Producers.Add(vmProducer.Producer);
                #endregion Creating Producer

                #region Creating linked application data
                var appUser = new ApplicationUser { UserName = vmProducer.Producer.Email, Email = vmProducer.Producer.Email };
                appUser.User = vmProducer.Producer;

                var result = await _userManager.CreateAsync(appUser, vmProducer.Producer.Email);
                if (result.Succeeded)
                {
                    //Add user role
                    result = await _userManager.AddToRoleAsync(appUser, vmProducer.UserRole.ToString());
                    //Add user type
                    result = await _userManager.AddToRoleAsync(appUser, Configurations.UserType.Producer.ToString());
                }
                #endregion Creating linked application data

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vmProducer);
        }

        // GET: Producer/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ApplicationUser appUser = _context.Users.First(x => x.Email == producer.Email);
            IList<string> roles = await _userManager.GetRolesAsync(appUser);
            string role = roles.FirstOrDefault(x => Configurations.GetRoles().Contains(x));
            return View(new ProducerViewModel(producer, (Configurations.Role)Enum.Parse(typeof(Configurations.Role), role)));
        }

        // POST: Producer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProducerViewModel vmProducer, IFormFile uploadFile, Configurations.Role UserRole)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    string uploads = Path.Combine(_environment.WebRootPath, Configurations.UserAvatarStockagePath);
                    //Deleting old image
                    string oldImage = Path.Combine(uploads, vmProducer.Producer.Avatar);
                    if (System.IO.File.Exists(oldImage) && vmProducer.Producer.Avatar != Path.Combine(Configurations.UserAvatarStockagePath, Configurations.DefaultFileName))
                        System.IO.File.Delete(Path.Combine(uploads, vmProducer.Producer.Avatar));
                    //Image uploading
                    string fileName = Guid.NewGuid().ToString() + "_" + ContentDispositionHeaderValue.Parse(uploadFile.ContentDisposition).FileName.Trim('"');
                    await uploadFile.SaveAsAsync(Path.Combine(uploads, fileName));
                    //Setting new value, saving
                    vmProducer.Producer.Avatar = Path.Combine(Configurations.UserAvatarStockagePath, fileName);
                }
                ApplicationUser appUser = _context.Users.First(x => x.Email == vmProducer.OriginalEmail);
                appUser.Email = vmProducer.Producer.Email;
                _context.Update(appUser);
                //Getting actual roles
                IList<string> roles = await _userManager.GetRolesAsync(appUser);
                if (!roles.Contains(UserRole.ToString()))
                {
                    string roleToRemove = roles.FirstOrDefault(x => Configurations.GetRoles().Contains(x));
                    await _userManager.RemoveFromRoleAsync(appUser, roleToRemove);
                    //Add user role
                    await _userManager.AddToRoleAsync(appUser, UserRole.ToString());
                }
                _context.Update(vmProducer.Producer);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vmProducer);
        }

        // GET: Producer/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
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
            ApplicationUser appUser = _context.Users.First(x => x.Email == producer.Email);
            IList<string> roles = await _userManager.GetRolesAsync(appUser);
            string role = roles.FirstOrDefault(x => Configurations.GetRoles().Contains(x));
            return View(new ProducerViewModel(producer, (Configurations.Role)Enum.Parse(typeof(Configurations.Role), role)));
        }

        // POST: Producer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Producer producer = _context.Producers.Single(m => m.Id == id);
            //Deleting image
            string uploads = Path.Combine(_environment.WebRootPath, Configurations.UserAvatarStockagePath);
            string image = Path.Combine(uploads, producer.Avatar);
            if (System.IO.File.Exists(image) && producer.Avatar != Path.Combine(Configurations.UserAvatarStockagePath, Configurations.DefaultFileName))
                System.IO.File.Delete(Path.Combine(uploads, producer.Avatar));
            //Delete App User
            ApplicationUser appUser = _context.Users.First(x => x.Email == producer.Email);
            _context.Users.Remove(appUser);
            //Delete User
            _context.Producers.Remove(producer);
            //Save
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
