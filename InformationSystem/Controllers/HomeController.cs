using InformationSystem.Data;
using InformationSystem.Data.Entity;
using InformationSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace InformationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<WebAppUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<WebAppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _context.Users.ToList();
            if (users.Count() < 1)
            {
                await createAdmin();
            }
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                if (user.IsAdmin)
                {
                    return RedirectToAction("AdminView");
                }
                else if (user.IsBarangayOfficial)
                {
                    return RedirectToAction("Index", "BarangayOfficialView");
                }
                else
                    return RedirectToAction("Index", "ResidentView");

            }
            return View();
        }
        private async Task<IActionResult> createAdmin() 
        {
            try
            {
                var date = DateTime.Now.AddYears(-21);
                var user = new WebAppUser { Name = "Admin User", UserName = "InformationSystem01@yopmail.com", EmailConfirmed = true, Email = "InformationSystem01@yopmail.com", BarangayId = 0, Gender = Data.Enum.Gender.Male, DateOfBirth = date, Age = 21, IsAdmin = true,
                BloodType = "Admin", Address = "Admin", PostalCode = "Admin", BirthPlace = "Admin", CivilStatus = Data.Enum.CivilStatus.Single, JobStatus = Data.Enum.JobStatus.GovernmentEmployee, BarangayUserId = "0", ImagePath = ""};
                var result = await _userManager.CreateAsync(user, "InformationSystem01@");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        public IActionResult AdminView()
        {
            ViewBag.Events = _context.Events.ToList();
            ViewBag.PendingUserCount = _context.Users.Where(x => !x.IsAdmin && !x.IsApproved).Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public float GetPendingUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (user.IsAdmin)
                return _context.Users.Where(x => !x.IsAdmin && !x.IsApproved).Count();
            else if (user.IsBarangayOfficial)
                return _context.Users.Where(x => !x.IsAdmin && !x.IsApproved && x.BarangayId == user.BarangayId).Count();
            else
                return 0;
        }

        public int GetNotification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notif = _context.Notification.Where(x => x.UserId == userId && !x.isRead).Count();
            return notif;
        }

        public float GetPendingComplaint()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.Complaint.Where(x => x.BarangayId == user.BarangayId && !x.isRead).Count();
        }

        public float GetPendingReports()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.Reports.Where(x => x.BarangayId == user.BarangayId && !x.isRead).Count();
        }

        public float GetPendingFeedBack()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.FeedBacks.Where(x => !x.isRead).Count();
        }

        public float GetPendingAccident()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.AccidentProne.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId && !x.IsRead).Count();
        }
    }
}
