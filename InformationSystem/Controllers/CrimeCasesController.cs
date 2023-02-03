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
using ClosedXML.Excel;
using InformationSystem.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace InformationSystem.Controllers
{
    public class CrimeCasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CrimeCasesController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: CrimeCases
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

            return View(await _context.CrimeCases.Include(h => h.Crime).Include(h => h.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync());
        }

        public async Task<IActionResult> GenerateCrimeCases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            return View(await _context.CrimeCases.Include(h => h.Crime).Include(h => h.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync());
        }

        // GET: CrimeCases/Create
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
            ViewBag.Crimes = new SelectList(_context.Crime, "CrimeId", "CrimeName");
            if (id == 0)
            {
                var crimeCases = new CrimeCases();
                return View(crimeCases);
            }
            else
            {
                return View(_context.CrimeCases.Where(x => x.CrimeCasesId == id).FirstOrDefault());
            }

        }

        // POST: CrimeCases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CrimeCasessId,CrimeId,Latitude,Longitude,UserId")] CrimeCases crimeCases)
        {

            if (ModelState.IsValid)
            {
                if (crimeCases.CrimeCasesId == 0)
                {
                    crimeCases.DateTime = DateTime.Now.AddHours(8);
                    _context.Add(crimeCases);
                }
                else
                    _context.Update(crimeCases);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(crimeCases);
        }

        // GET: CrimeCases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var crimeCases = await _context.CrimeCases.FindAsync(id);
            _context.CrimeCases.Remove(crimeCases);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Approve(int crimeCasesId)
        {
            var reports = _context.CrimeCases.Where(x => x.CrimeCasesId == crimeCasesId).FirstOrDefault();
            reports.Status = ReportStatus.Approve;
            _context.Update(reports);

            var notif = new Notification()
            {
                UserId = reports.UserId,
                Title = "Your report Crime Case is approved!",
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
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Crime_Cases_Brgy_Alfonso.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Crime Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Crime";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var crimeCases = _context.CrimeCases.Include(x => x.Crime).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Crime_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }
            else if (userBarangay.BarangayName.Contains("Green Village"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Crime_Cases_Brgy_Green_Village.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Crime Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Crime";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var crimeCases = _context.CrimeCases.Include(x => x.Crime).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Crime_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }
            else if (userBarangay.BarangayName.Contains("Minane"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Crime_Cases_Brgy_Minane.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Crime Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Crime";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var crimeCases = _context.CrimeCases.Include(x => x.Crime).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Crime_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }
            else if (userBarangay.BarangayName.Contains("San Nicolas Pob"))
            {
                using (var workbook = new XLWorkbook(projectRootPath + "\\documents\\Crime_Cases_Brgy_San_Nicolas_Pob.xlsx"))
                {
                    var worksheet = workbook.Worksheets.Worksheet("Crime Cases");
                    var currentRow = 10;

                    worksheet.Cell(currentRow, 1).Value = "User";
                    worksheet.Cell(currentRow, 2).Value = "Crime";
                    worksheet.Cell(currentRow, 3).Value = "Status";
                    worksheet.Cell(currentRow, 4).Value = "Date And Time";


                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 3).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;

                    var crimeCases = _context.CrimeCases.Include(x => x.Crime).Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToList();


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
                        "Crime_Cases_" + DateTime.Now.AddHours(8) + ".xlsx"
                        );
                    }
                }
            }

            return null;

        }
    }
}
