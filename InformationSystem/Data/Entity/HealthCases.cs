using InformationSystem.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class HealthCases
    {
        [Key]
        public int HealthCasesId { get; set; }
        [DisplayName("Disease Name")]
        public int DiseaseId { get; set; }
        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
        public Disease Disease { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateTime { get; set; }
        [DisplayName("Status")]
        public ReportStatus Status { get; set; }
    }
}
