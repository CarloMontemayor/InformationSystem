using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum ReportStatus
    {
        [Display(Name = "All")]
        All,
        [Display(Name = "Pending")]
        Pending,
        [Display(Name = "Verified")]
        Approve,
        [Display(Name = "Rejected")]
        Rejected
    }
}
