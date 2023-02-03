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
using ClosedXML.Excel;
using InformationSystem.Data.Enum;
using System.IO;
using InformationSystem.Data.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using InformationSystem.Services;

namespace InformationSystem.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Complaints
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

            var complaints = await _context.Complaint.Include(h => h.User).Where(x => x.UserId == userId).ToListAsync();
            foreach (var complaint in complaints)
            {
                if (!string.IsNullOrEmpty(complaint.Victim))
                {
                    var victim = _context.Users.Find(complaint.Victim);
                    if (victim != null)
                        complaint.Victim = victim.Name;
                }
            }

            return View(await _context.Complaint.Include(h => h.User).Where(x => x.UserId == userId).ToListAsync());
        }

        // GET: Complaints/Create
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
            ViewBag.Name = user.Name;
            ViewBag.Now = DateTime.Now.AddHours(8).ToString("MM/dd/yyyy");
            ViewBag.Disease = new SelectList(_context.Disease, "DiseaseId", "DiseaseName");
            ViewBag.Crimes = new SelectList(_context.Crime, "CrimeId", "CrimeName");
            ViewBag.Victim = new SelectList(_context.Users, "Id", "Name");
            if (id == 0)
            {
                var complaints = new ComplaintDto();
                complaints.UserId = userId;
                complaints.BarangayId = user.BarangayId;
                return View(complaints);
            }
            else
            {
                var complaint = _context.Complaint.Where(x => x.ComplaintId == id).FirstOrDefault();
                var complaints = new ComplaintDto();
                complaints.ComplaintId = complaint.ComplaintId;
                complaints.ComplaintType = complaint.ComplaintType;
                complaints.Victim = complaint.Victim;
                complaints.Respondents = complaint.Respondents;
                complaints.Date = complaint.Date;
                complaints.TimeOfIncidents = complaint.TimeOfIncidents;
                complaints.IncidentLocation = complaint.IncidentLocation;
                complaints.Latitude = complaint.Latitude;
                complaints.Longitude = complaint.Longitude;
                complaints.IncidentType = complaint.IncidentType;
                complaints.Detals = complaint.Detals;
                complaints.DiseaseId = complaint.DiseaseId;
                complaints.CrimeId = complaint.CrimeId;
                complaints.BarangayId = user.BarangayId;
                complaints.isRead = complaint.isRead;
                complaints.ImagePathString = complaint.ImagePath;

                return View(complaints);
            }
        }

        // POST: Complaints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(ComplaintDto complaint)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            var splits = complaint.LatitudeLongitude.Split(',').ToList<string>();
            complaint.Latitude = double.Parse(splits[0]);
            complaint.Longitude = double.Parse(splits[1]);
            if (complaint.ComplaintId == 0)
            {

                if (ModelState.IsValid)
                {
                    string imageFileName = UploadedFile(complaint);
                    var complaints = new Complaint();
                    complaints.ComplaintId = complaint.ComplaintId;
                    complaints.ComplaintType = complaint.ComplaintType;
                    complaints.Victim = complaint.Victim;
                    complaints.Respondents = complaint.Respondents;
                    complaints.Date = complaint.Date;
                    complaints.TimeOfIncidents = complaint.TimeOfIncidents;
                    complaints.IncidentLocation = complaint.IncidentLocation;
                    complaints.Latitude = complaint.Latitude;
                    complaints.Longitude = complaint.Longitude;
                    complaints.IncidentType = complaint.IncidentType;
                    complaints.Detals = complaint.Detals;
                    complaints.DiseaseId = complaint.DiseaseId;
                    complaints.CrimeId = complaint.CrimeId;
                    complaints.BarangayId = user.BarangayId;
                    complaints.isRead = complaint.isRead;
                    complaints.ImagePath = imageFileName;
                    complaints.UserId = userId;
                    complaints.DateReported = complaint.DateReported;

                    _context.Add(complaints);
                    var notif = new Notification()
                    {
                        UserId = complaints.Victim,
                        Title = "You have been reported for " + EnumExtensions.GetDisplayName(complaints.ComplaintType),
                        DateTime = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Add(notif);
                    Complaints(complaints);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                if (complaint.ImagePath == null)
                {
                    var complaintDb = _context.Complaint.AsNoTracking().SingleOrDefault(x => x.ComplaintId == complaint.ComplaintId);
                    var complaints = new Complaint();
                    complaints.ComplaintId = complaint.ComplaintId;
                    complaints.ComplaintType = complaint.ComplaintType;
                    complaints.Victim = complaint.Victim;
                    complaints.Respondents = complaint.Respondents;
                    complaints.Date = complaint.Date;
                    complaints.TimeOfIncidents = complaint.TimeOfIncidents;
                    complaints.IncidentLocation = complaint.IncidentLocation;
                    complaints.Latitude = complaint.Latitude;
                    complaints.Longitude = complaint.Longitude;
                    complaints.IncidentType = complaint.IncidentType;
                    complaints.Detals = complaint.Detals;
                    complaints.DiseaseId = complaint.DiseaseId;
                    complaints.CrimeId = complaint.CrimeId;
                    complaints.BarangayId = user.BarangayId;
                    complaints.isRead = complaint.isRead;
                    complaints.ImagePath = complaintDb.ImagePath;
                    complaints.UserId = userId;

                    _context.Update(complaints);

                    var notif = new Notification()
                    {
                        UserId = complaints.Victim,
                        Title = "You have been reported for " + EnumExtensions.GetDisplayName(complaints.ComplaintType),
                        DateTime = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Add(notif);

                    Complaints(complaints);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    string imageFileName = UploadedFile(complaint);
                    var complaints = new Complaint();
                    complaints.ComplaintId = complaint.ComplaintId;
                    complaints.ComplaintType = complaint.ComplaintType;
                    complaints.Victim = complaint.Victim;
                    complaints.Respondents = complaint.Respondents;
                    complaints.Date = complaint.Date;
                    complaints.TimeOfIncidents = complaint.TimeOfIncidents;
                    complaints.IncidentLocation = complaint.IncidentLocation;
                    complaints.Latitude = complaint.Latitude;
                    complaints.Longitude = complaint.Longitude;
                    complaints.IncidentType = complaint.IncidentType;
                    complaints.Detals = complaint.Detals;
                    complaints.DiseaseId = complaint.DiseaseId;
                    complaints.CrimeId = complaint.CrimeId;
                    complaints.BarangayId = user.BarangayId;
                    complaints.isRead = complaint.isRead;
                    complaints.ImagePath = imageFileName;
                    complaints.UserId = userId;

                    _context.Update(complaints);

                    var notif = new Notification()
                    {
                        UserId = complaints.Victim,
                        Title = "You have been reported for " + EnumExtensions.GetDisplayName(complaints.ComplaintType),
                        DateTime = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Add(notif);

                    Complaints(complaints);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(complaint);
        }

        private string UploadedFile(ComplaintDto model)
        {
            string uniqueFileName = null;

            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public void Complaints(Complaint complaint)
        {
            if (complaint.DiseaseId != 0 && complaint.DiseaseId != null)
            {
                HealthCases cases = new HealthCases()
                {
                    DiseaseId = complaint.DiseaseId.Value,
                    UserId = complaint.UserId,
                    Latitude = complaint.Latitude,
                    Longitude = complaint.Longitude,
                    Status = ReportStatus.Pending,
                    DateTime = DateTime.Now.AddHours(8)
                };

                var logs = new AuditLog()
                {
                    UserId = complaint.UserId,
                    Title = "User Added Complaint Health",
                    DateRequested = DateTime.UtcNow.AddHours(8)
                };
                _context.Add(logs);
                _context.Add(cases);
            }
            else if (complaint.CrimeId != 0 && complaint.CrimeId != null)
            {
                CrimeCases cases = new CrimeCases()
                {
                    CrimeId = complaint.CrimeId.Value,
                    UserId = complaint.UserId,
                    Latitude = complaint.Latitude,
                    Longitude = complaint.Longitude,
                    Status = ReportStatus.Pending,
                    DateTime = DateTime.Now.AddHours(8)
                };

                var logs = new AuditLog()
                {
                    UserId = complaint.UserId,
                    Title = "User Added Complaint Crime",
                    DateRequested = DateTime.UtcNow.AddHours(8)
                };
                _context.Add(logs);
                _context.Add(cases);
            }
        }


        // GET: Complaints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var complaint = await _context.Complaint.FindAsync(id);
            _context.Complaint.Remove(complaint);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BarangayViewComplaintReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            var complaints = await _context.Complaint.Where(x => x.BarangayId == user.BarangayId).ToListAsync();
            foreach (var complaint in complaints)
            {
                complaint.isRead = true;
                _context.Update(complaint);
            }

            await _context.SaveChangesAsync();

            foreach (var complaint in complaints)
            {
                if (!string.IsNullOrEmpty(complaint.Victim))
                {
                    var victim = _context.Users.Find(complaint.Victim);
                    if (victim != null)
                        complaint.Victim = victim.Name;
                }
            }

            var applicationDbContext = _context.Complaint.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> DeleteBarangayViewComplaintReport(int? id)
        {
            var complaint = await _context.Complaint.FindAsync(id);
            _context.Complaint.Remove(complaint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BarangayViewComplaintReport));
        }

        public IActionResult GenerateReport(ComplaintReport complaintReport)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(complaintReport.ToString() + " Complaints");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Cases";
                worksheet.Cell(currentRow, 2).Value = "Total record";
                worksheet.Cell(currentRow, 3).Value = "Percentage";


                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 3).Style.Font.Bold = true;

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                int number = 365;
                var complaints = new List<Complaint>();
                if (complaintReport == ComplaintReport.All)
                    complaints.AddRange(_context.Complaint.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId).ToList());
                else if (complaintReport == ComplaintReport.Month)
                {
                    complaints.AddRange(_context.Complaint.Include(r => r.User).Where(r => r.Date.Month == DateTime.Now.Month && r.BarangayId == user.BarangayId).ToList());
                    number = 30;
                }
                else if (complaintReport == ComplaintReport.Year)
                    complaints.AddRange(_context.Complaint.Include(r => r.User).Where(r => r.Date.Year == DateTime.Now.Year && r.BarangayId == user.BarangayId).ToList());

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Health";

                decimal record = complaints.Where(x => x.ComplaintType == ComplaintType.Disease).Count();
                worksheet.Cell(currentRow, 2).Value = record;
                decimal total = Math.Round(record / number, 2);
                decimal totalRecord = Math.Round(total * 100, 2);
                worksheet.Cell(currentRow, 3).Value = totalRecord.ToString() + "%";

                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Crime";

                decimal recordHealth = complaints.Where(x => x.ComplaintType == ComplaintType.Crime).Count();
                worksheet.Cell(currentRow, 2).Value = recordHealth;
                decimal totalHealth = Math.Round(recordHealth / number, 2);
                decimal totalRecordHealth = Math.Round(totalHealth * 100, 2);
                worksheet.Cell(currentRow, 3).Value = totalRecordHealth.ToString() + "%";

                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Complaints_" + complaintReport.ToString() + ".xlsx"
                    );
                }
            }
        }
        public IActionResult BarangayViewComplaintGenerate(ComplaintReport complaintReport)
        {
            ViewBag.Report = complaintReport.GetHashCode();
            return View();

        }

        public List<decimal> GenerateDataReport(ComplaintReport complaintReport)
        {
            var result = new List<decimal>();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            int number = 365;
            var complaints = new List<Complaint>();
            if (complaintReport == ComplaintReport.All)
                complaints.AddRange(_context.Complaint.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId).ToList());
            else if (complaintReport == ComplaintReport.Month)
            {
                complaints.AddRange(_context.Complaint.Include(r => r.User).Where(r => r.Date.Month == DateTime.Now.Month && r.BarangayId == user.BarangayId).ToList());
                number = 30;
            }
            else if (complaintReport == ComplaintReport.Year)
                complaints.AddRange(_context.Complaint.Include(r => r.User).Where(r => r.Date.Year == DateTime.Now.Year && r.BarangayId == user.BarangayId).ToList());

            decimal record = complaints.Where(x => x.ComplaintType == ComplaintType.Disease).Count();
            decimal total = Math.Round(record / number, 2);
            decimal totalRecord = Math.Round(total * 100, 2);
            result.Add(totalRecord);

            decimal recordHealth = complaints.Where(x => x.ComplaintType == ComplaintType.Crime).Count();
            decimal totalHealth = Math.Round(recordHealth / number, 2);
            decimal totalRecordHealth = Math.Round(totalHealth * 100, 2);
            result.Add(totalRecordHealth);
            return result;
        }

        public async Task<IActionResult> ViewImage(int id)
        {
            var complaint = await _context.Complaint.FindAsync(id);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            var imagesFile = uploadsFolder + "\\" + complaint.ImagePath;

            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(imagesFile, out contentType);


            return PhysicalFile(imagesFile, contentType);
        }
    }
}
