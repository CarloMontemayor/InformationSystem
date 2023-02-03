using InformationSystem.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class BarangayOfficial
    {
        [Key]
        public int BarangayOfficalId { get; set; }
        public int BarangayId { get; set; }
        public BarangayPosition Position { get; set; }
        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }

        [DisplayName("Year Of Service")]
        public string YearOfService { get; set; }
    }
}
