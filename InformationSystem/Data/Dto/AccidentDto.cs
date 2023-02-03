using InformationSystem.Data.Entity;
using InformationSystem.Data.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Dto
{
    public class AccidentDto
    {
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
        [DisplayName("Latitude | Longitude")]
        public string LatitudeLongitude { get; set; }


        [DisplayName("Proof")]
        public IFormFile ImagePath { get; set; }
        public string ImagePathString { get; set; }
    }
}
