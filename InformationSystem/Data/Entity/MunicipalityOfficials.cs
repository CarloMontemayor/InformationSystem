using InformationSystem.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class MunicipalityOfficials
    {
        [Key]
        public int MunicipalityOfficialsId { get; set; }
        [Required]
        [DisplayName("Official Name")]
        public string OfficialName { get; set; }
        [Required]
        [DisplayName("Position")]
        public MunicipalityPosition Position { get; set; }
        [Required]
        [DisplayName("Elected Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ElectedDate { get; set; }
        [Required]
        [DisplayName("Address")]
        public string Address { get; set; }
    }
}
