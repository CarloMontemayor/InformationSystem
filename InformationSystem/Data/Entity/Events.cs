using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace InformationSystem.Data.Entity
{
    public class Events
    {
        [Key]
        public int EventsId { get; set; }
        [DisplayName("Announcement Name")]
        public string EventName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Where { get; set; }
        public DateTime When { get; set; }
        public string Time { get; set; }
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
    }
}
