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
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notification
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifs = await _context.Notification.Where(x => x.UserId == userId).ToListAsync();
            foreach (var notif in notifs)
            {
                notif.isRead = true;
                _context.Update(notif);
            }
            await _context.SaveChangesAsync();
            return View(await _context.Notification.Where(x => x.UserId == userId).ToListAsync());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var notificaton = await _context.Notification.FindAsync(id);
            _context.Notification.Remove(notificaton);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
