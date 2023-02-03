using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InformationSystem.Data;
using InformationSystem.Data.Entity;
using ClosedXML.Excel;
using System.IO;
using InformationSystem.Services;

namespace InformationSystem.Controllers
{
    public class BarangayController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarangayController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Barangay
        public async Task<IActionResult> Index()
        {
            return View(await _context.Barangay.ToListAsync());
        }

        // GET: Barangay/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Barangay());
            else
                return View(_context.Barangay.Find(id));
        }

        // POST: Barangay/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("BarangayId,BarangayName")] Barangay barangay)
        {
            if (ModelState.IsValid)
            {
                if (barangay.BarangayId == 0)
                    _context.Add(barangay);
                else
                    _context.Update(barangay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(barangay);
        }

        // GET: Barangay/Delete/5
        // GET: Municipality/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var barangay = await _context.Barangay.FindAsync(id);
            _context.Barangay.Remove(barangay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult GenerateHealth(long barangayId)
        {

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Health Cases");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "User";
                worksheet.Cell(currentRow, 2).Value = "Disease";
                worksheet.Cell(currentRow, 3).Value = "Status";
                worksheet.Cell(currentRow, 4).Value = "Date And Time";

                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                var healthCases = _context.HealthCases.Include(x => x.Disease).Include(x => x.User).Where(x => x.User.BarangayId == barangayId).ToList();


                foreach (var healthCase in healthCases)
                {

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = healthCase.User.Name;
                    worksheet.Cell(currentRow, 2).Value = healthCase.Disease.DiseaseName;
                    worksheet.Cell(currentRow, 3).Value = healthCase.Status.GetDisplayName();
                    worksheet.Cell(currentRow, 4).Value = healthCase.DateTime;
                }
                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Health_Cases_.xlsx"
                    );
                }
            }

        }

        public IActionResult GenerateCrime(long barangayId)
        {

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Crime Cases");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "User";
                worksheet.Cell(currentRow, 2).Value = "Crime";
                worksheet.Cell(currentRow, 3).Value = "Status";
                worksheet.Cell(currentRow, 4).Value = "Date And Time";

                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                var crimeCases = _context.CrimeCases.Include(x => x.Crime).Include(x => x.User).Where(x => x.User.BarangayId == barangayId).ToList();


                foreach (var crimCase in crimeCases)
                {

                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = crimCase.User.Name;
                    worksheet.Cell(currentRow, 2).Value = crimCase.Crime.CrimeName;
                    worksheet.Cell(currentRow, 3).Value = crimCase.Status.GetDisplayName();
                    worksheet.Cell(currentRow, 4).Value = crimCase.DateTime;

                }
                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Crime_Cases_.xlsx"
                    );
                }
            }

        }
    }
}
