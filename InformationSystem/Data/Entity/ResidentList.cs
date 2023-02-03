using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace InformationSystem.Data.Entity
{
    public class ResidentList
    {
        [Key]
        public int ResidentListId { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Barangay ID No.")]
        public string BarangayNumber { get; set; }
        public string Address { get; set; }
        public string Dates { get; set; }
        [Required]
        [DisplayName("Barangay")]
        public int BarangayId { get; set; }
    }
}
