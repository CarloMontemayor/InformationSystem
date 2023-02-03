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
using Spire.Doc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using InformationSystem.Services;

namespace InformationSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ReportsController(ApplicationDbContext context,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.Reports.Include(r => r.User).Where(x => x.UserId == userId);
            ViewBag.Disabled = false;
            var complaints = _context.Complaint.Where(x => x.Victim == userId && x.ComplaintType == ComplaintType.Crime).Any();
            if (complaints)
                ViewBag.Disabled = true;

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reports/Create
        public IActionResult AddOrEdit(int id = 0)
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); ;
            ViewBag.DateNow = DateTime.UtcNow.AddHours(8);

            ViewBag.Disabled = false;
            var complaints = _context.Complaint.Where(x => x.Victim == userId && x.ComplaintType == ComplaintType.Crime).Any();
            if (complaints)
                ViewBag.Disabled = true;

            if (id == 0)
                return View(new Reports());
            else
                return View(_context.Reports.Find(id));
        }

        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("ReportsId,ReportType,UserId,MonthlyIncome,DateRequested,Reason,BusinessName,MotherName,FatherName")] Reports reports)
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.DateNow = DateTime.Now;
            if (ModelState.IsValid)
            {
                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var barangayId = _context.Users.Where(x => x.Id == user).Select(x => x.BarangayId).FirstOrDefault();
                reports.Status = Data.Enum.ReportStatus.Pending;
                reports.BarangayId = barangayId;
                if (reports.ReportsId == 0)
                {
                    var logs = new AuditLog()
                    {
                        UserId = reports.UserId,
                        Title = "User Added Report: " + reports.ReportType.GetDisplayName(),
                        DateRequested = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Add(reports);
                    _context.Add(logs);

                }
                else
                {
                    var logs = new AuditLog()
                    {
                        UserId = reports.UserId,
                        Title = "User Updated Report: " + reports.ReportType.GetDisplayName(),
                        DateRequested = DateTime.UtcNow.AddHours(8)
                    };
                    _context.Update(reports);
                    _context.Add(reports);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reports);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var reports = await _context.Reports.FindAsync(id);
            _context.Reports.Remove(reports);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BarangayDelete(int? id)
        {
            var reports = await _context.Reports.FindAsync(id);
            _context.Reports.Remove(reports);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(BarangayViewReport));
        }

        public async Task<IActionResult> BarangayViewReport(ReportStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();


            var reports = await _context.Reports.Where(x => x.BarangayId == user.BarangayId).ToListAsync();
            foreach (var report in reports)
            {
                report.isRead = true;
                _context.Update(report);
            }
            await _context.SaveChangesAsync();

            if (status == ReportStatus.All)
            {
                var applicationDbContext = _context.Reports.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.Reports.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId && x.Status == status);
                return View(await applicationDbContext.ToListAsync());
            }

        }

        public async Task<IActionResult> Approve(int reportId)
        {
            var reports = _context.Reports.Where(x => x.ReportsId == reportId).FirstOrDefault();
            reports.Status = ReportStatus.Approve;
            _context.Update(reports);

            var notif = new Notification()
            {
                UserId = reports.UserId,
                Title = "Your request: " + EnumExtensions.GetDisplayName(reports.ReportType) + " is approved! You may claim it anytime.",
                DateTime = DateTime.UtcNow.AddHours(8)
            };
            _context.Add(notif);
            await _context.SaveChangesAsync();
            return RedirectToAction("BarangayViewReport");

        }

        public async Task<IActionResult> ResidentViewReport(ReportStatus status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (status == ReportStatus.All)
            {
                var applicationDbContext = _context.Reports.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId && x.UserId == user.Id);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.Reports.Include(r => r.User).Where(x => x.BarangayId == user.BarangayId && x.Status == status && x.UserId == user.Id);
                return View(await applicationDbContext.ToListAsync());
            }

        }

        public IActionResult GenerateIndigency(int reportId)
        {
            var report = _context.Reports.Where(x => x.ReportsId == reportId).FirstOrDefault();
            var user = _context.Users.Where(x => x.Id == report.UserId).FirstOrDefault();
            var barangay = _context.Barangay.Where(x => x.BarangayId == user.BarangayId).FirstOrDefault();
            var barangayOfficials = _context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayId == barangay.BarangayId).ToList();
            Document doc = new Document();
            string projectRootPath = _hostingEnvironment.WebRootPath;
            string docs = projectRootPath + "documents";
            doc.LoadFromFile(projectRootPath + "\\documents\\Indigency.docx");

            doc.Replace("Barangay_Name", barangay.BarangayName, true, true);

            var barangayNumber = 1;
            foreach (var barangayOfficial in barangayOfficials)
            {
                switch (barangayOfficial.Position)
                {
                    case BarangayPosition.BarangayChairman:
                        doc.Replace("Barangay_Captain", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Kagawad:
                        doc.Replace("Barangay_Kagawad" + barangayNumber, barangayOfficial.User.Name.ToUpper(), true, true);
                        barangayNumber++;
                        break;
                    case BarangayPosition.Secretary:
                        doc.Replace("Barangay_Secretary", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Treasurer:
                        doc.Replace("Barangay_Treasurer", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    default:
                        // code block
                        break;
                }
            }
            //Remove
            doc.Replace("Barangay_Kagawad1", "", true, true);
            doc.Replace("Barangay_Kagawad2", "", true, true);
            doc.Replace("Barangay_Kagawad3", "", true, true);
            doc.Replace("Barangay_Kagawad4", "", true, true);
            doc.Replace("Barangay_Kagawad5", "", true, true);
            //
            doc.Replace("Resident_Name", user.Name.ToUpper(), true, true);
            doc.Replace("Barangay_Day", DateTime.Now.Day.ToString(), true, true);
            doc.Replace("Barangay_Month", DateTime.Now.ToString("MMMM"), true, true);
            doc.Replace("Barangay_Year", DateTime.Now.ToString("yyyy"), true, true);

            doc.SaveToFile("FindandReplace.docx", FileFormat.Docx2013);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                doc.SaveToStream(ms1, FileFormat.Docx2013);
                //save to byte array
                toArray = ms1.ToArray();
            }
            Stream stream = new MemoryStream(toArray);
            return File(fileStream: stream, contentType: "application/msword", fileDownloadName: "Indigency_" + user.Name + ".docx");
        }

        public IActionResult GenerateClearance(int reportId)
        {
            var report = _context.Reports.Where(x => x.ReportsId == reportId).FirstOrDefault();
            var user = _context.Users.Where(x => x.Id == report.UserId).FirstOrDefault();
            var barangay = _context.Barangay.Where(x => x.BarangayId == user.BarangayId).FirstOrDefault();
            var barangayOfficials = _context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayId == barangay.BarangayId).ToList();
            Document doc = new Document();
            string projectRootPath = _hostingEnvironment.WebRootPath;
            string docs = projectRootPath + "documents";
            doc.LoadFromFile(projectRootPath + "\\documents\\Clearance.docx");

            doc.Replace("Barangay_Name", barangay.BarangayName, true, true);

            var barangayNumber = 1;
            foreach (var barangayOfficial in barangayOfficials)
            {
                switch (barangayOfficial.Position)
                {
                    case BarangayPosition.BarangayChairman:
                        doc.Replace("Barangay_Captain", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Kagawad:
                        doc.Replace("Barangay_Kagawad" + barangayNumber, barangayOfficial.User.Name.ToUpper(), true, true);
                        barangayNumber++;
                        break;
                    case BarangayPosition.Secretary:
                        doc.Replace("Barangay_Secretary", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Treasurer:
                        doc.Replace("Barangay_Treasurer", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    default:
                        // code block
                        break;
                }
            }
            //Remove
            doc.Replace("Barangay_Kagawad1", "", true, true);
            doc.Replace("Barangay_Kagawad2", "", true, true);
            doc.Replace("Barangay_Kagawad3", "", true, true);
            doc.Replace("Barangay_Kagawad4", "", true, true);
            doc.Replace("Barangay_Kagawad5", "", true, true);
            //

            doc.Replace("Resident_Name", user.Name.ToUpper(), true, true);
            doc.Replace("Resident_BirthPlace", user.Name.ToUpper(), true, true);
            doc.Replace("Barangay_Day", DateTime.Now.Day.ToString(), true, true);
            doc.Replace("Barangay_Month", DateTime.Now.ToString("MMMM"), true, true);
            doc.Replace("Barangay_Year", DateTime.Now.ToString("yyyy"), true, true);

            doc.SaveToFile("FindandReplace.docx", FileFormat.Docx2013);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                doc.SaveToStream(ms1, FileFormat.Docx2013);
                //save to byte array
                toArray = ms1.ToArray();
            }
            Stream stream = new MemoryStream(toArray);
            return File(fileStream: stream, contentType: "application/msword", fileDownloadName: "Clearance_" + user.Name + ".docx");
        }

        public IActionResult GenerateCertificate(int reportId)
        {
            var report = _context.Reports.Where(x => x.ReportsId == reportId).FirstOrDefault();
            var user = _context.Users.Where(x => x.Id == report.UserId).FirstOrDefault();
            var barangay = _context.Barangay.Where(x => x.BarangayId == user.BarangayId).FirstOrDefault();
            var barangayOfficials = _context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayId == barangay.BarangayId).ToList();
            Document doc = new Document();
            string projectRootPath = _hostingEnvironment.WebRootPath;
            string docs = projectRootPath + "documents";
            doc.LoadFromFile(projectRootPath + "\\documents\\Certification.docx");

            doc.Replace("Barangay_Name", barangay.BarangayName, true, true);

            var barangayNumber = 1;
            foreach (var barangayOfficial in barangayOfficials)
            {
                switch (barangayOfficial.Position)
                {
                    case BarangayPosition.BarangayChairman:
                        doc.Replace("Barangay_Captain", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Kagawad:
                        doc.Replace("Barangay_Kagawad" + barangayNumber, barangayOfficial.User.Name.ToUpper(), true, true);
                        barangayNumber++;
                        break;
                    case BarangayPosition.Secretary:
                        doc.Replace("Barangay_Secretary", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Treasurer:
                        doc.Replace("Barangay_Treasurer", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    default:
                        // code block
                        break;
                }
            }
            //Remove
            doc.Replace("Barangay_Kagawad1", "", true, true);
            doc.Replace("Barangay_Kagawad2", "", true, true);
            doc.Replace("Barangay_Kagawad3", "", true, true);
            doc.Replace("Barangay_Kagawad4", "", true, true);
            doc.Replace("Barangay_Kagawad5", "", true, true);
            //

            doc.Replace("Resident_Name", user.Name.ToUpper(), true, true);

            doc.Replace("Resident_Age", user.Age.ToString(), true, true);
            doc.Replace("Resident_BirthPlace", user.BirthPlace, true, true);

            doc.Replace("Barangay_Day", DateTime.Now.Day.ToString(), true, true);
            doc.Replace("Barangay_Month", DateTime.Now.ToString("MMMM"), true, true);
            doc.Replace("Barangay_Year", DateTime.Now.ToString("yyyy"), true, true);

            doc.SaveToFile("FindandReplace.docx", FileFormat.Docx2013);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                doc.SaveToStream(ms1, FileFormat.Docx2013);
                //save to byte array
                toArray = ms1.ToArray();
            }
            Stream stream = new MemoryStream(toArray);
            return File(fileStream: stream, contentType: "application/msword", fileDownloadName: "Certification_" + user.Name + ".docx");
        }

        public IActionResult GenerateBusinessPermit(int reportId)
        {
            var report = _context.Reports.Where(x => x.ReportsId == reportId).FirstOrDefault();
            var user = _context.Users.Where(x => x.Id == report.UserId).FirstOrDefault();
            var barangay = _context.Barangay.Where(x => x.BarangayId == user.BarangayId).FirstOrDefault();
            var barangayOfficials = _context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayId == barangay.BarangayId).ToList();
            Document doc = new Document();
            string projectRootPath = _hostingEnvironment.WebRootPath;
            string docs = projectRootPath + "documents";
            doc.LoadFromFile(projectRootPath + "\\documents\\Permit.docx");

            doc.Replace("Barangay_Name", barangay.BarangayName, true, true);

            var barangayNumber = 1;
            foreach (var barangayOfficial in barangayOfficials)
            {
                switch (barangayOfficial.Position)
                {
                    case BarangayPosition.BarangayChairman:
                        doc.Replace("Barangay_Captain", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Kagawad:
                        doc.Replace("Barangay_Kagawad" + barangayNumber, barangayOfficial.User.Name.ToUpper(), true, true);
                        barangayNumber++;
                        break;
                    case BarangayPosition.Secretary:
                        doc.Replace("Barangay_Secretary", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Treasurer:
                        doc.Replace("Barangay_Treasurer", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    default:
                        // code block
                        break;
                }
            }
            //Remove
            doc.Replace("Barangay_Kagawad1", "", true, true);
            doc.Replace("Barangay_Kagawad2", "", true, true);
            doc.Replace("Barangay_Kagawad3", "", true, true);
            doc.Replace("Barangay_Kagawad4", "", true, true);
            doc.Replace("Barangay_Kagawad5", "", true, true);
            //

            doc.Replace("Resident_Business", report.BusinessName.ToUpper(), true, true);
            doc.Replace("Resident_Name", user.Name.ToUpper(), true, true);
            doc.Replace("Barangay_Day", DateTime.Now.Day.ToString(), true, true);
            doc.Replace("Barangay_Month", DateTime.Now.ToString("MMMM"), true, true);
            doc.Replace("Barangay_Year", DateTime.Now.ToString("yyyy"), true, true);

            doc.SaveToFile("FindandReplace.docx", FileFormat.Docx2013);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                doc.SaveToStream(ms1, FileFormat.Docx2013);
                //save to byte array
                toArray = ms1.ToArray();
            }
            Stream stream = new MemoryStream(toArray);
            return File(fileStream: stream, contentType: "application/msword", fileDownloadName: "Certification_" + user.Name + ".docx");
        }

        public IActionResult GenerateSingle(int reportId)
        {
            var report = _context.Reports.Where(x => x.ReportsId == reportId).FirstOrDefault();
            var user = _context.Users.Where(x => x.Id == report.UserId).FirstOrDefault();
            var barangay = _context.Barangay.Where(x => x.BarangayId == user.BarangayId).FirstOrDefault();
            var barangayOfficials = _context.BarangayOfficial.Include(x => x.User).Where(x => x.BarangayId == barangay.BarangayId).ToList();
            Document doc = new Document();
            string projectRootPath = _hostingEnvironment.WebRootPath;
            string docs = projectRootPath + "documents";
            doc.LoadFromFile(projectRootPath + "\\documents\\Singleness.docx");

            doc.Replace("Barangay_Name", barangay.BarangayName, true, true);

            var barangayNumber = 1;
            foreach (var barangayOfficial in barangayOfficials)
            {
                switch (barangayOfficial.Position)
                {
                    case BarangayPosition.BarangayChairman:
                        doc.Replace("Barangay_Captain", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Kagawad:
                        doc.Replace("Barangay_Kagawad" + barangayNumber, barangayOfficial.User.Name.ToUpper(), true, true);
                        barangayNumber++;
                        break;
                    case BarangayPosition.Secretary:
                        doc.Replace("Barangay_Secretary", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    case BarangayPosition.Treasurer:
                        doc.Replace("Barangay_Treasurer", barangayOfficial.User.Name.ToUpper(), true, true);
                        break;
                    default:
                        // code block
                        break;
                }
            }
            //Remove
            doc.Replace("Barangay_Kagawad1", "", true, true);
            doc.Replace("Barangay_Kagawad2", "", true, true);
            doc.Replace("Barangay_Kagawad3", "", true, true);
            doc.Replace("Barangay_Kagawad4", "", true, true);
            doc.Replace("Barangay_Kagawad5", "", true, true);
            //
            doc.Replace("Resident_Age", user.Age.ToString(), true, true);
            doc.Replace("Resident_BirthPlace", user.BirthPlace, true, true);

            doc.Replace("Resident_Mother", report.MotherName.ToUpper(), true, true);
            doc.Replace("Resident_Father", report.FatherName.ToUpper(), true, true);
            doc.Replace("Resident_Name", user.Name.ToUpper(), true, true);
            doc.Replace("Barangay_Day", DateTime.Now.Day.ToString(), true, true);
            doc.Replace("Barangay_Month", DateTime.Now.ToString("MMMM"), true, true);
            doc.Replace("Barangay_Year", DateTime.Now.ToString("yyyy"), true, true);

            doc.SaveToFile("FindandReplace.docx", FileFormat.Docx2013);

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                doc.SaveToStream(ms1, FileFormat.Docx2013);
                //save to byte array
                toArray = ms1.ToArray();
            }
            Stream stream = new MemoryStream(toArray);
            return File(fileStream: stream, contentType: "application/msword", fileDownloadName: "Certification_" + user.Name + ".docx");
        }
    }
}
