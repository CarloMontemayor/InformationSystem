using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum ReportType
    {
        [Display(Name = "Barangay Indigency")]
        BarangayIndigency,
        [Display(Name = "Barangay Clearance")]
        BarangayClearance,
        [Display(Name = "Barangay Certification")]
        BarangayCertificate,
        [Display(Name = "Barangay Business Permit")]
        BarangayPermit,
        [Display(Name = "Barangay Certificate of singleness")]
        BarangaySingle
    }
}
