using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Enum
{
    public enum MunicipalityPosition
    {
        Mayor,
        [Display(Name = "Vice Mayor")]
        ViceMayor,
        Councilor
    }
}
