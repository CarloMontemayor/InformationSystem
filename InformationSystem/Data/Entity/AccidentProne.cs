using InformationSystem.Data.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InformationSystem.Data.Entity
{
    public class AccidentProne
    {
        [Key]
        public int AccidentProneId { get; set; }
        [DisplayName("Details")]
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
        public ReportStatus Status { get; set; }

        public string Respondent { get; set; }
        public string Where { get; set; }
        public DateTime When { get; set; }
        public string Time { get; set; }

        public bool IsRead { get; set; }


        [Display(Name = "Proof")]
        public string ImagePath { get; set; }
    }
}
