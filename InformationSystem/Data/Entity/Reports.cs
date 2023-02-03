using InformationSystem.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class Reports
    {
        [Key]
        public int ReportsId { get; set; }
        [DisplayName("Report Type")]
        public ReportType ReportType { get; set;}
        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
        [DisplayName("Date Requested")]
        public DateTime DateRequested { get; set; }
        [DisplayName("Status")]
        public ReportStatus Status { get; set; }
        [DisplayName("Reason")]
        public string Reason { get; set; }
        [DisplayName("Date Approve")]
        public DateTime DateApprove { get; set; }
        public int BarangayId { get; set; }
        [DisplayName("Monthly Income")]
        public float MonthlyIncome { get; set; }
        public bool isRead { get; set; }

        [DisplayName("Business Name")]
        public string BusinessName { get; set; }
        [DisplayName("Mother Name")]
        public string MotherName { get; set; }
        [DisplayName("Father Name")]
        public string FatherName { get; set; }
    }
}
