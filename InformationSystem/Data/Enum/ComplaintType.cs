using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum ComplaintType
    {
        [Display(Name = "Health")]
        Disease,
        [Display(Name = "Crime")]
        Crime
    }
}
