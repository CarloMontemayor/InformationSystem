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
using InformationSystem.Data.Enum;
using InformationSystem.Services;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace InformationSystem.Controllers
{
    public class HealthCasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HealthCasesController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

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

            return View(await _context.HealthCases.Include(h => h.Disease).Include(h => h.User).Where(x => x.User.BarangayId == user.BarangayId).OrderBy(x => x.Status).ToListAsync());
        }

        public async Task<IActionResult> GenerateHealthCases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            return View(await _context.HealthCases.Include(h => h.Disease).Include(h => h.User).Where(x => x.User.BarangayId == user.BarangayId).OrderBy(x => x.Status).ToListAsync());
        }

        // GET: HealthCases/Create
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

            ViewBag.Users = new SelectList(_context.Users.Where(x => !x.IsAdmin && x.IsApproved), "Id", "Name");
            ViewBag.Diseases = new SelectList(_context.Disease, "DiseaseId", "DiseaseName");
            if (id == 0)
            {
                var healthCases = new HealthCases();
                return View(healthCases);
            }
            else
            {
                return View(_context.HealthCases.Where(x => x.HealthCasesId == id).FirstOrDefault());
            }

        }

        // POST: HealthCases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("HealthCasesId,DiseaseId,Latitude,Longitude,UserId")] HealthCases healthCases)
        {

            if (ModelState.IsValid)
            {
                if (healthCases.HealthCasesId == 0)
                {
                    healthCases.DateTime = DateTime.Now.AddHours(8);
                    _context.Add(healthCases);
                }
                else
                    _context.Update(healthCases);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(healthCases);
        }


        // GET: HealthCases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var healthCases = await _context.HealthCases.FindAsync(id);
            _context.HealthCases.Remove(healthCases);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Approve(int healthCasesId)
        {
            var reports = _context.HealthCases.Where(x => x.HealthCasesId == healthCasesId).FirstOrDefault();
            reports.Status = ReportStatus.Approve;
            _context.Update(reports);

            var notif = new Notification()
            {
                UserId = reports.UserId,
                Title = "Your report Health Case is approved!",
                DateTime = DateTime.UtcNow.AddHours(8)
            };
            _context.Add(notif);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult GenerateReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var userBarangay = _context.Barangay.Where(x => x.BarangayId == user.BarangayId).FirstOrDefault();
            string projectRootPath = _hostingEnvironment.WebRootPath;
            string docs = projectRootPath + "documents";


            if (userBarangay.BarangayName.Contains("Alfonso"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Health_Cases_Brgy_Alfonso.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Health Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Disease";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var healthCases = _context.HealthCases.Include(x => x.Disease).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Health_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }
            else if (userBarangay.BarangayName.Contains("Green Village"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Health_Cases_Brgy_Green_Village.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Health Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Disease";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var healthCases = _context.HealthCases.Include(x => x.Disease).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Health_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }
            else if (userBarangay.BarangayName.Contains("Minane"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Health_Cases_Brgy_Minane.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Health Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Disease";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var healthCases = _context.HealthCases.Include(x => x.Disease).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Health_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }

            else if (userBarangay.BarangayName.Contains("San Nicolas Pob"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Health_Cases_Brgy_San_Nicolas_Pob.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Health Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Disease";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var healthCases = _context.HealthCases.Include(x => x.Disease).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Health_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }

            return null;
        }
    }
}
