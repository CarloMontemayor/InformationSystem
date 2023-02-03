using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class Municipality
    {
        [Key]
        public int MunicipalityId { get; set; }
        [Required]
        [DisplayName("Municipality Name")]
        public string MunicipalityName { get; set; }
    }
}
