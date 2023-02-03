using InformationSystem.Data.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InformationSystem.Data.Entity
{
    public class CrimeCases
    {
        [Key]
        public int CrimeCasesId { get; set; }
        public int CrimeId { get; set; }
        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
        public Crime Crime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DateTime { get; set; }
        [DisplayName("Status")]
        public ReportStatus Status { get; set; }
    }
}
