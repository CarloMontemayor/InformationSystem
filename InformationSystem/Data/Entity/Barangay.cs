using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class Barangay
    {
        [Key]
        public int BarangayId { get; set; }
        [Required]
        [DisplayName("Barangay Name")]
        public string BarangayName { get; set; }
    }
}
