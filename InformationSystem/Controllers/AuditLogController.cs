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
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            return View(await _context.AuditLogs.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync());
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            var auditLogs = await _context.AuditLogs.FindAsync(id);
            _context.AuditLogs.Remove(auditLogs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
