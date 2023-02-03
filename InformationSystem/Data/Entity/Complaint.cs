using InformationSystem.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }
        [DisplayName("Complainants")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }

        [DisplayName("Complaint Type")]
        public ComplaintType ComplaintType { get; set; }
        [DisplayName("Person To Be Reported")]
        public string Victim { get; set; }
        [DisplayName("Respondents")]
        public string Respondents { get; set; }
        [DisplayName("Date")]
        public DateTime Date { get; set; }
        [DisplayName("Time Of Incidents")]
        public string TimeOfIncidents { get; set; }

        [DisplayName("Incident Location")]
        public string IncidentLocation { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [DisplayName("Incident Type")]
        public string IncidentType { get; set; }
        [DisplayName("Date Reported")]
        public DateTime DateReported { get; set; }
        [DisplayName("Details")]
        public string Detals { get; set; }
        [DisplayName("Disease")]
        public int? DiseaseId { get; set; }
        [DisplayName("Crime")]
        public int? CrimeId { get; set; }

        public int BarangayId { get; set; }
        public bool isRead { get; set; }

        [Display(Name = "Proof")]
        public string ImagePath { get; set; }
    }
}
