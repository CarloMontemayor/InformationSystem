using InformationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InformationSystem.Controllers
{
    public class BarangayOfficialViewController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BarangayOfficialViewController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var barangayOfficial = _context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayId == user.BarangayId && x.Position != Data.Enum.BarangayPosition.Resident).ToList();
            ViewBag.PendingUserCount = _context.Users.Where(x => !x.IsAdmin && !x.IsApproved && x.BarangayId == user.BarangayId).Count();
            return View(barangayOfficial);
        }

        public int GetTeenAgerData()
        {
            return _context.Users.Where(x => !x.IsAdmin).Count(x => x.Age >= 13 && x.Age <= 19);
        }
        public int GetAdultData()
        {
            return _context.Users.Where(x => !x.IsAdmin).Count(x => x.Age >= 20 && x.Age <= 60);
        }
        public int GetSeniorData()
        {
            return _context.Users.Where(x => !x.IsAdmin).Count(x => x.Age >= 61);
        }
    }
}
