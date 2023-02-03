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
    public class BarangayEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarangayEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return View(await _context.Events.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync());
        }

        // GET: Events/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            var events = new Events();
            if (id == 0)
            {
                events.When = DateTime.UtcNow.AddHours(8);
                return View(events);
            }
            else
                return View(_context.Events.Find(id));
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("EventsId,EventName,When,Where,Time,Latitude,Longitude")] Events events)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                events.UserId = userId;
                if (events.EventsId == 0)
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
            var events = await _context.Events.FindAsync(id);
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
