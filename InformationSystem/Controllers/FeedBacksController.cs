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
    public class FeedBacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedBacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FeedBacks
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

            return View(await _context.FeedBacks.Include(x => x.User).Where(x => x.UserId == userId).ToListAsync());
        }
        // GET: FeedBacks/Create
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

            ViewBag.UserId = userId;
            if (id == 0)
                return View(new FeedBack());
            else
                return View(_context.FeedBacks.Find(id));
        }

        // POST: FeedBacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("FeedBackId,Details,Rating,UserId,DetailsSuggestion,RatingService")] FeedBack feedBack)
        {
            if (ModelState.IsValid)
            {
                if (feedBack.FeedBackId == 0)
                {
                    var logs = new AuditLog()
                    {
                        UserId = feedBack.UserId,
                        Title = "User Added FeedBack",
                        DateRequested = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Add(logs);
                    _context.Add(feedBack);
                }

                else
                {
                    var logs = new AuditLog()
                    {
                        UserId = feedBack.UserId,
                        Title = "User Updated FeedBack",
                        DateRequested = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Update(logs);
                    _context.Update(feedBack);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feedBack);
        }
        // GET: FeedBacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var feedback = await _context.FeedBacks.FindAsync(id);
            _context.FeedBacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BarangayViewFeedback()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            var feedbacks = await _context.FeedBacks.ToListAsync();
            foreach (var feedback in feedbacks)
            {
                feedback.isRead = true;
                _context.Update(feedback);
            }
            await _context.SaveChangesAsync();
            var applicationDbContext = _context.FeedBacks.Include(r => r.User).Where(x => x.User.BarangayId == user.BarangayId);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> DeleteBarangayFeedBack(int? id)
        {
            var feedback = await _context.FeedBacks.FindAsync(id);
            _context.FeedBacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BarangayViewFeedback));
        }
    }
}
