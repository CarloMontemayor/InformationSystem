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
using InformationSystem.Data.Dto;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using InformationSystem.Data.Enum;
using Microsoft.AspNetCore.StaticFiles;

namespace InformationSystem.Controllers
{
    public class AccidentProneController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccidentProneController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: AccidentProne
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
            {
                ViewBag.IsBarangayOfficial = true;
                var accidents = await _context.AccidentProne.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync();
                foreach (var accident in accidents)
                {
                    accident.IsRead = true;
                    _context.Update(accident);
                }
                await _context.SaveChangesAsync();
                return View(accidents);
            }
            return View(await _context.AccidentProne.Include(x => x.User).Where(x => x.User.Id == user.Id).ToListAsync());
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

            ViewBag.Now = DateTime.Now.AddHours(8).ToString("MM/dd/yyyy");
            if (id == 0)
                return View(new AccidentDto());
            else
            {
                var complaint = _context.AccidentProne.Where(x => x.AccidentProneId == id).FirstOrDefault();
                var complaints = new AccidentDto();
                complaints.AccidentProneId = complaint.AccidentProneId;
                complaints.ImagePathString = complaint.ImagePath;
                complaints.Longitude = complaint.Longitude;
                complaints.Latitude = complaint.Latitude;
                complaints.Name = complaint.Name;

                complaints.Respondent = complaint.Respondent;
                complaints.Where = complaint.Where;
                complaints.Time = complaint.Time;
                complaints.When = complaint.When;

                return View(_context.AccidentProne.Find(id));
            }
        }

        // POST: AccidentProne/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(AccidentDto accidentProne)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            var splits = accidentProne.LatitudeLongitude.Split(',').ToList<string>();
            accidentProne.Latitude = double.Parse(splits[0]);
            accidentProne.Longitude = double.Parse(splits[1]);

            if (accidentProne.AccidentProneId == 0)
            {
                if (ModelState.IsValid)
                {
                    string imageFileName = UploadedFile(accidentProne);

                    accidentProne.UserId = userId;
                    var accident = new AccidentProne()
                    {
                        Name = accidentProne.Name,
                        UserId = accidentProne.UserId,
                        Latitude = accidentProne.Latitude,
                        Longitude = accidentProne.Longitude,
                        Status = ReportStatus.Pending,
                        ImagePath = imageFileName,

                        Respondent = accidentProne.Respondent,
                        When = accidentProne.When,
                        Time = accidentProne.Time,
                        Where = accidentProne.Where
                    };

                    _context.Add(accident);

                    var logs = new AuditLog()
                    {
                        UserId = accidentProne.UserId,
                        Title = "User Reported Accident",
                        DateRequested = DateTime.UtcNow.AddHours(8)
                    };

                    _context.Add(logs);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else 
            {
                if (accidentProne.ImagePath == null)
                {
                    var accidentProneDb = _context.AccidentProne.AsNoTracking().SingleOrDefault(x => x.AccidentProneId == accidentProne.AccidentProneId);
                    string imageFileName = UploadedFile(accidentProne);
                    accidentProne.UserId = userId;
                    var accident = new AccidentProne()
                    {
                        Name = accidentProne.Name,
                        UserId = accidentProne.UserId,
                        Latitude = accidentProne.Latitude,
                        Longitude = accidentProne.Longitude,
                        Status = accidentProne.Status,
                        ImagePath = imageFileName,

                        Respondent = accidentProne.Respondent,
                        When = accidentProne.When,
                        Time = accidentProne.Time,
                        Where = accidentProne.Where
                    };

                    _context.Update(accident);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    string imageFileName = UploadedFile(accidentProne);
                    accidentProne.UserId = userId;
                    var accident = new AccidentProne()
                    {
                        Name = accidentProne.Name,
                        UserId = accidentProne.UserId,
                        Latitude = accidentProne.Latitude,
                        Longitude = accidentProne.Longitude,
                        Status = accidentProne.Status,
                        ImagePath = imageFileName,

                        Respondent = accidentProne.Respondent,
                        When = accidentProne.When,
                        Time = accidentProne.Time,
                        Where = accidentProne.Where
                    };

                    _context.Update(accident);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
           
            return View(accidentProne);
        }

        private string UploadedFile(AccidentDto model)
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

        public async Task<IActionResult> Delete(int id)
        {
            var accidentProne = await _context.AccidentProne.FindAsync(id);
            _context.AccidentProne.Remove(accidentProne);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<List<AccidentProne>> GetAccidentProneMap()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return await _context.AccidentProne.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync();
        }

        public async Task<IActionResult> Approve(int accidentProneId)
        {
            var reports = _context.AccidentProne.Where(x => x.AccidentProneId == accidentProneId).FirstOrDefault();
            reports.Status = ReportStatus.Approve;
            _context.Update(reports);

            var notif = new Notification()
            {
                UserId = reports.UserId,
                Title = "Your report Accident is approved!",
                DateTime = DateTime.UtcNow.AddHours(8)
            };
            _context.Add(notif);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ViewImage(int id)
        {
            var complaint = await _context.AccidentProne.FindAsync(id);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            var imagesFile = uploadsFolder + "\\" + complaint.ImagePath;

            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(imagesFile, out contentType);


            return PhysicalFile(imagesFile, contentType);
        }

    }
}
