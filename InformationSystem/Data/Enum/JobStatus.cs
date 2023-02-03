using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum JobStatus
    {
        [Display(Name = "None")]
        None,
        [Display(Name = "Student")]
        Student,
        [Display(Name = "Private Employee")]
        PrivateEmployee,
        [Display(Name = "Government Employee")]
        GovernmentEmployee,
        [Display(Name = "Self Employed")]
        SelfEmployed,
        [Display(Name = "Retired")]
        Retired
    }
}
