using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class Disease
    {
        [Key]
        public int DiseaseId { get; set; }
        [Required]
        [DisplayName("Disease Name")]
        public string DiseaseName { get; set; }
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
    }
}
