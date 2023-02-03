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
    public class MunicipalityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MunicipalityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Municipality
        public async Task<IActionResult> Index()
        {
            return View(await _context.Municipality.ToListAsync());
        }

        // GET: Municipality/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            if(id ==0)
                return View(new Municipality());
            else
                return View(_context.Municipality.Find(id));
        }

        // POST: Municipality/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("MunicipalityId,MunicipalityName")] Municipality municipality)
        {
            if (ModelState.IsValid)
            {
                if(municipality.MunicipalityId == 0)
                    _context.Add(municipality);
                else
                    _context.Update(municipality);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(municipality);
        }


        // GET: Municipality/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var municipality = await _context.Municipality.FindAsync(id);
            _context.Municipality.Remove(municipality);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
