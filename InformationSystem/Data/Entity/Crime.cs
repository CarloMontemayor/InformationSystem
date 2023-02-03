using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class Crime
    {
        [Key]
        public int CrimeId { get; set; }
        [Required]
        [DisplayName("Crime Name")]
        public string CrimeName { get; set; }
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
    }
}
