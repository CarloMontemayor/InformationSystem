using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InformationSystem.Data;
using InformationSystem.Data.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InformationSystem.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<WebAppUser> _userManager;
        private readonly SignInManager<WebAppUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(
            UserManager<WebAppUser> userManager,
            SignInManager<WebAppUser> signInManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Name { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Image")]
            public IFormFile ImagePath { get; set; }


            [Required]
            [Display(Name = "Barangay ID Picture")]
            public IFormFile ImageBarangayPath { get; set; }
        }

        private async Task LoadAsync(WebAppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Name = user.Name,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            ViewData["IsAdmin"] = false;
            ViewData["IsBarangayOfficial"] = false;
            if (user.IsAdmin)
                ViewData["IsAdmin"] = true;
            else if (user.IsBarangayOfficial)
                ViewData["IsBarangayOfficial"] = true;
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ViewData["IsAdmin"] = false;
            ViewData["IsBarangayOfficial"] = false;
            if (user.IsAdmin)
                ViewData["IsAdmin"] = true;
            else if (user.IsBarangayOfficial)
                ViewData["IsBarangayOfficial"] = true;

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            string imageFileName = UploadedFile(Input.ImagePath);
            string imageBarangayName = UploadedFile(Input.ImageBarangayPath);

            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }
            user.ImagePath = imageFileName;
            user.ImageBarangayPath = imageBarangayName;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private string UploadedFile(IFormFile imagePath)
        {
            string uniqueFileName = null;

            if (imagePath != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProfileImage");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + imagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
