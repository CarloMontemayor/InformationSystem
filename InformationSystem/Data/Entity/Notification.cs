using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InformationSystem.Data.Entity
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [DisplayName("Title")]
        public string Title { get; set; }
        public bool isRead { get; set; }
        public DateTime DateTime { get; set; }
        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
    }
}
