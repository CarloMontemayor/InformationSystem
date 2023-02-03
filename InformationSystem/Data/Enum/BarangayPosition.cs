using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum BarangayPosition
    {
        [Display(Name = "Barangay Chairman")]
        BarangayChairman,
        [Display(Name = "Barangay Kagawad")]
        Kagawad,
        [Display(Name = "SK Chairperson")]
        SKChairperson,
        [Display(Name = "Barangay Secretary")]
        Secretary,
        [Display(Name = "Barangay Health Worker")]
        HealthWorker,
        [Display(Name = "Barangay Treasurer")]
        Treasurer,
        [Display(Name = "Barangay Police")]
        Police,
        [Display(Name = "Resident")]
        Resident
    }
}
