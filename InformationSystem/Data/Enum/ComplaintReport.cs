using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum ComplaintReport
    {
        [Display(Name = "All")]
        All,
        [Display(Name = "This Month")]
        Month,
        [Display(Name = "This Year")]
        Year
    }
}
