using InformationSystem.Data;
using InformationSystem.Data.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InformationSystem.Controllers
{
    public class PendingUserBarangayController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PendingUserBarangayController(ApplicationDbContext context, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var pendingUsers = await _context.Users.Where(x => !x.IsAdmin && !x.IsApproved && x.BarangayId == user.BarangayId).ToListAsync();
            var userDtos = new List<UserDto>();
            foreach (var users in pendingUsers) 
            {
                var barangay = _context.Barangay.Where(y => y.BarangayId == users.BarangayId).FirstOrDefault();
                var hasImage = !string.IsNullOrEmpty(user.ImageBarangayPath);
                var dto = new UserDto()
                {
                    UserId = users.Id,
                    Name = users.Name,
                    Email = users.Email,
                    BarangayName = barangay.BarangayName,
                    hasBarangayID = hasImage
                };
                userDtos.Add(dto);
            }
            return View(userDtos);
        }

        public async Task<IActionResult> Approved(string userId)
        {
            if (userId == "" || userId == null)
            {
                throw new Exception("User not found!");
            }
            else
            {
                var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
                user.IsApproved = true;
                _context.Update(user);
                await _context.SaveChangesAsync();

                await _emailSender.SendEmailAsync(user.Email, "You have been approved!",
                $"Hi, <br> You have been approved! You may now visit the system.");
                return RedirectToAction("Index");
            }

        }
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewImage(string id)
        {
            var user = await _context.Users.FindAsync(id);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImage");
            var imagesFile = uploadsFolder + "\\" + user.ImageBarangayPath;

            string contentType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(imagesFile, out contentType);


            return PhysicalFile(imagesFile, contentType);
        }
    }
}
