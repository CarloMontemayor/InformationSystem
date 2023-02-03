using InformationSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InformationSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Where(x => !x.IsAdmin).ToListAsync());
        }

        public async Task<IActionResult> BarangayUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            return View(await _context.Users.Where(x => !x.IsAdmin && x.IsApproved && x.BarangayId == user.BarangayId).ToListAsync());
        }
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
            return RedirectToAction(nameof(BarangayUser));
        }

        public async Task<IActionResult> ViewImage(string id)
        {
            var complaint = await _context.Users.FindAsync(id);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImage");
            var imagesFile = uploadsFolder + "\\" + complaint.ImageBarangayPath;

            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(imagesFile, out contentType);


            return PhysicalFile(imagesFile, contentType);
        }
    }
}
