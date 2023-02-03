using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Data;
using InformationSystem.Data.Entity;

namespace InformationSystem.Controllers
{
    public class MunicipalityOfficialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MunicipalityOfficialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MunicipalityOfficials
        public async Task<IActionResult> Index()
        {
            return View(await _context.MunicipalityOfficials.ToListAsync());
        }

        // GET: MunicipalityOfficials/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            ViewBag.Now = DateTime.Now.ToString("MM/dd/yyyy");
            if (id == 0)
                return View(new MunicipalityOfficials());
            else
                return View(_context.MunicipalityOfficials.Find(id));
        }

        // POST: MunicipalityOfficials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("MunicipalityOfficialsId,OfficialName,Position,ElectedDate,Address")] MunicipalityOfficials municipalityOfficials)
        {
            if (ModelState.IsValid)
            {
                if (municipalityOfficials.MunicipalityOfficialsId == 0)
                    _context.Add(municipalityOfficials);
                else
                    _context.Update(municipalityOfficials);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(municipalityOfficials);
        }
      
        // GET: MunicipalityOfficials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var municipality = await _context.MunicipalityOfficials.FindAsync(id);
            _context.MunicipalityOfficials.Remove(municipality);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
