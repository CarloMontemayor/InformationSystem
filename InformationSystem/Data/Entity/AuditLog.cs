using InformationSystem.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Entity
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }

        public string Title { get; set; }

        [DisplayName("Date Time")]
        public DateTime DateRequested { get; set; }
    }
}
