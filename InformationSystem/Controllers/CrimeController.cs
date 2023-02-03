using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Data;
using InformationSystem.Data.Enum;
using InformationSystem.Data.Entity;
using System.Security.Claims;

namespace InformationSystem.Controllers
{
    public class CrimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CrimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Crime
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            return View(await _context.Crime.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync());
        }

        // GET: Crime/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            if (id == 0)
                return View(new Crime());
            else
                return View(_context.Crime.Find(id));
        }

        // POST: Crime/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CrimeId,CrimeName")] Crime crime)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                crime.UserId = userId;
                if (crime.CrimeId == 0)
                    _context.Add(crime);
                else
                    _context.Update(crime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(crime);
        }

        // GET: Crime/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var crime = await _context.Crime.FindAsync(id);
            _context.Crime.Remove(crime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
