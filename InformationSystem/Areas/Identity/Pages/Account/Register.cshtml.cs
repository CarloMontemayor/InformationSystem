using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using InformationSystem.Data;
using InformationSystem.Data.Entity;
using InformationSystem.Data.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace InformationSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<WebAppUser> _signInManager;
        private readonly UserManager<WebAppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RegisterModel(
            UserManager<WebAppUser> userManager,
            SignInManager<WebAppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public SelectList Barangay { get; set; }
        public DateTime Dates { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            [Display(Name = "Barangay")]
            public int BarangayId { get; set; }
            [Required]
            [Display(Name = "Gender")]
            public Gender Gender { get; set; }
            [Required]
            [Display(Name = "Date Of Birth")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime DateOfBirth { get; set; }
            [Required]
            [Display(Name = "Age")]
            public int Age { get; set; }
            //OtherInfo
            [Required]
            [Display(Name = "Blood Type")]
            public string BloodType { get; set; }
            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }
            [Required]
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }
            [Required]
            [Display(Name = "Birth Place")]
            public string BirthPlace { get; set; }
            [Required]
            [Display(Name = "Civil Status")]
            public CivilStatus CivilStatus { get; set; }
            [Required]
            [Display(Name = "Job Status")]
            public JobStatus JobStatus { get; set; }
            [Required]
            [Display(Name = "Barangay ID Number")]
            public string BarangayUserId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "PhoneNumber")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Image")]
            public IFormFile ImagePath { get; set; }

            [Required]
            [Display(Name = "Barangay ID Picture")]
            public IFormFile ImageBarangayPath { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Barangay = new SelectList(_context.Barangay, "BarangayId", "BarangayName");
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Dates = DateTime.Now;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                string imageFileName = UploadedFile(Input.ImagePath);
                string imageBarangayName = UploadedFile(Input.ImageBarangayPath);

                var user = new WebAppUser { Name = Input.Name, UserName = Input.Email, Email = Input.Email, BarangayId = Input.BarangayId, Gender = Input.Gender, DateOfBirth = Input.DateOfBirth, Age = Input.Age,
                BirthPlace = Input.BirthPlace, BloodType = Input.BloodType, Address = Input.Address, PostalCode = Input.PostalCode, BarangayUserId = Input.BarangayUserId, CivilStatus = Input.CivilStatus, JobStatus = Input.JobStatus,
                    PhoneNumber = Input.PhoneNumber, ImagePath = imageFileName, ImageBarangayPath = imageBarangayName };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var verify = _context.ResidentLists.Where(x => x.BarangayId == Input.BarangayId && x.BarangayNumber.ToLower().Contains(Input.BarangayUserId)).FirstOrDefault();

                    if (verify == null)
                    {
                        ModelState.AddModelError(string.Empty, "Barangay ID Number does not exists!");
                        return Page();
                    }

                    if (user.Age <= 17)
                    {
                        ModelState.AddModelError(string.Empty, "You must be 18 and above to sign up.");
                        return Page();
                    }
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. Wait until the Admin/Official approve you to access the system.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Barangay = new SelectList(_context.Barangay, "BarangayId", "BarangayName");
            Dates = DateTime.Now;
            // If we got this far, something failed, redisplay form
            return Page();
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
