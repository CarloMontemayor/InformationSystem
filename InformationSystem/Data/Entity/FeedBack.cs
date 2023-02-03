using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace InformationSystem.Data.Entity
{
    public class FeedBack
    {
        [Key]
        public int FeedBackId { get; set; }
        [DisplayName("Do you have any problems or errors when using the system?")]
        public string Details { get; set; }
        [DisplayName("How do you rate our overall performance of the system?")]
        public int Rating { get; set; }
        [DisplayName("Name")]
        public string UserId { get; set; }
        public WebAppUser User { get; set; }
        public bool isRead { get; set; }
        [DisplayName("What are your comments and suggestions?")]
        public string DetailsSuggestion { get; set; }
        [DisplayName("What are your thoughts in the service we provided?")]
        public int RatingService { get; set; }
    }
}
