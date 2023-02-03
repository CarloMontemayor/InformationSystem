using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Data;
using InformationSystem.Data.Entity;
using System.Security.Claims;

namespace InformationSystem.Controllers
{
    public class ResidentListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResidentListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                ViewBag.IsAdmin = false;
                ViewBag.IsBarangayOfficial = false;
                if (user.IsAdmin)
                    ViewBag.IsAdmin = true;
                else if (user.IsBarangayOfficial)
                    ViewBag.IsBarangayOfficial = true;

                if (user.IsAdmin)
                    return View(await _context.ResidentLists.ToListAsync());
                else
                    return View(await _context.ResidentLists.Where(x => x.BarangayId == user.BarangayId).ToListAsync());
            }
        }

        // GET: Events/Create
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

            ViewBag.Barangay = new SelectList(_context.Barangay, "BarangayId", "BarangayName");
            var events = new ResidentList();
            if (id == 0)
            {
                return View(events);
            }
            else
                return View(_context.ResidentLists.Find(id));
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("ResidentListId,Name,BarangayId,BarangayNumber,Address,Dates")] ResidentList events)
        {
            ViewBag.Barangay = new SelectList(_context.Barangay, "BarangayId", "BarangayName");
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                if(events.BarangayId == 0)
                    events.BarangayId = user.BarangayId;
                if (events.ResidentListId == 0)
                    _context.Add(events);
                else
                    _context.Update(events);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(events);
        }
        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var events = await _context.ResidentLists.FindAsync(id);
            _context.ResidentLists.Remove(events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
