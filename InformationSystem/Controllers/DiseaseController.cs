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
    public class DiseaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiseaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Disease
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

            return View(await _context.Disease.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync());
        }

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
                return View(new Disease());
            else
                return View(_context.Disease.Find(id));
        }

        // POST: Disease/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("DiseaseId,DiseaseName")] Disease disease)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                disease.UserId = userId;
                if (disease.DiseaseId == 0)
                    _context.Add(disease);
                else
                    _context.Update(disease);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(disease);
        }

        // GET: Disease/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var crime = await _context.Disease.FindAsync(id);
            _context.Disease.Remove(crime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
