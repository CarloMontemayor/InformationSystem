using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum CivilStatus
    {
        [Display(Name = "Single")]
        Single,
        [Display(Name = "Married")]
        Married,
        [Display(Name = "Divorced")]
        Divorced,
        [Display(Name = "Separated")]
        Separated,
        [Display(Name = "Widowed ")]
        Widowed
    }
}
