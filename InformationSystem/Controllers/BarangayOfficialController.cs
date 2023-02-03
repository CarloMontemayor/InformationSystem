using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Data;
using InformationSystem.Data.Entity;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace InformationSystem.Controllers
{
    public class BarangayOfficialController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BarangayOfficialController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: BarangayOfficial
        public IActionResult Index(int barangayId)
        {
            ViewBag.BarangayId = barangayId;
            ViewBag.BarangayName = "";
            if (barangayId != 0)
                ViewBag.BarangayName = _context.Barangay.Find(barangayId).BarangayName;

            var barangayOfficialList = new List<BarangayOfficial>();
            var users = _context.Users.Where(x => x.BarangayId == barangayId).ToList();
            foreach (var user in users)
            {
                var official = _context.BarangayOfficial.Include(x => x.User).Where(x => x.UserId == user.Id && x.BarangayId == barangayId).FirstOrDefault();
                if (official != null)
                {
                    barangayOfficialList.Add(official);
                }
                else
                {
                    var brgyOfficial = new BarangayOfficial();
                    brgyOfficial.BarangayId = user.BarangayId;
                    brgyOfficial.UserId = user.Id;
                    brgyOfficial.User = user;
                    brgyOfficial.Position = Data.Enum.BarangayPosition.Resident;
                    barangayOfficialList.Add(brgyOfficial);
                }
            }
            return View(barangayOfficialList);
        }

        // GET: BarangayOfficial/Create
        public IActionResult AddOrEdit(int id, int barangayId, string userId)
        {
            ViewBag.Users = new SelectList(_context.Users.Where(x => !x.IsAdmin && x.IsApproved), "Id", "Name");
            if (id == 0)
            {
                var barangayOfficial = new BarangayOfficial();
                barangayOfficial.BarangayId = barangayId;
                if (!string.IsNullOrEmpty(userId))
                {
                    barangayOfficial.UserId = userId;
                    barangayOfficial.Position = Data.Enum.BarangayPosition.Resident;
                }

                return View(barangayOfficial);
            }
            else
                return View(_context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayOfficalId == id).FirstOrDefault());
        }

        // POST: BarangayOfficial/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("BarangayId,UserId,BarangayOfficalId,Position,YearOfService")] BarangayOfficial barangayOfficial)
        {
            if (ModelState.IsValid)
            {
                if (barangayOfficial.BarangayOfficalId == 0)
                {

                    var user = _context.Users.Find(barangayOfficial.UserId);
                    if (barangayOfficial.Position != Data.Enum.BarangayPosition.Resident)
                        user.IsBarangayOfficial = true;
                    _context.Add(barangayOfficial);
                    _context.Update(user);
                }
                else {
                    _context.Update(barangayOfficial);
                    if (barangayOfficial.Position == Data.Enum.BarangayPosition.Resident)
                    {
                        var user = _context.Users.Find(barangayOfficial.UserId);
                        user.IsBarangayOfficial = false;
                        _context.Update(user);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "BarangayOfficial", new { barangayId = barangayOfficial.BarangayId });
            }
            return View(barangayOfficial);
        }



        // GET: BarangayOfficial/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                var barangayOfficials = _context.BarangayOfficial.Where(x => x.UserId == users.Id).ToList();
                foreach (var barangayOfficial in barangayOfficials)
                    _context.BarangayOfficial.Remove(barangayOfficial);

                var HealthCases = _context.HealthCases.Where(x => x.UserId == users.Id).ToList();
                foreach (var HealthCase in HealthCases)
                    _context.HealthCases.Remove(HealthCase);

                var CrimeCases = _context.CrimeCases.Where(x => x.UserId == users.Id).ToList();
                foreach (var CrimeCase in CrimeCases)
                    _context.CrimeCases.Remove(CrimeCase);

                var Reports = _context.Reports.Where(x => x.UserId == users.Id).ToList();
                foreach (var Report in Reports)
                    _context.Reports.Remove(Report);

                var Complaints = _context.Complaint.Where(x => x.UserId == users.Id).ToList();
                foreach (var Complaint in Complaints)
                    _context.Complaint.Remove(Complaint);

                var Notifications = _context.Notification.Where(x => x.UserId == users.Id).ToList();
                foreach (var Notification in Notifications)
                    _context.Notification.Remove(Notification);

                var AuditLogss = _context.AuditLogs.Where(x => x.UserId == users.Id).ToList();
                foreach (var AuditLogs in AuditLogss)
                    _context.AuditLogs.Remove(AuditLogs);

                var FeedBackss = _context.FeedBacks.Where(x => x.UserId == users.Id).ToList();
                foreach (var FeedBacks in FeedBackss)
                    _context.FeedBacks.Remove(FeedBacks);

                var accidents = _context.AccidentProne.Where(x => x.UserId == users.Id).ToList();
                foreach (var accident in accidents)
                    _context.AccidentProne.Remove(accident);

                var crimes = _context.Crime.Where(x => x.UserId == users.Id).ToList();
                foreach (var crime in crimes)
                    _context.Crime.Remove(crime);

                var diseases = _context.Disease.Where(x => x.UserId == users.Id).ToList();
                foreach (var disease in diseases)
                    _context.Disease.Remove(disease);

                var events = _context.Events.Where(x => x.UserId == users.Id).ToList();
                foreach (var eventsss in events)
                    _context.Events.Remove(eventsss);
            }
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "BarangayOfficial", new { barangayId = users.BarangayId });
        }

        public async Task<IActionResult> ViewImage(string id)
        {
            var user = await _context.Users.FindAsync(id);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImage");
            var imagesFile = uploadsFolder + "\\" + user.ImageBarangayPath;

            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(imagesFile, out contentType);


            return PhysicalFile(imagesFile, contentType);
        }
    }
}
