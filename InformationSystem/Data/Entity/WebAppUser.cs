using InformationSystem.Data.Enum;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class WebAppUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [Required]
        [DisplayName("Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [DisplayName("Barangay")]
        public int BarangayId { get; set; }
        [Required]
        [DisplayName("Gender")]
        public Gender Gender { get; set; }
        [Required]
        [DisplayName("Age")]
        public int Age { get; set; }
        public bool IsBarangayOfficial { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsApproved { get; set; }
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
        [Display(Name = "Barangay ID")]
        public string BarangayUserId { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        [Display(Name = "BarangayID")]
        public string ImageBarangayPath { get; set; }
    }
}
