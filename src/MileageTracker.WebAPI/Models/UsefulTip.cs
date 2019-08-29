using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MileageTracker.WebAPI.Models
{
    [Table("UsefulTip")]
    public class UsefulTip
    {
        [Key]
        public int UsefulTipId { get; set; }
        [Display(Name = "Tip Title")]
        public string Title { get; set; }
        [Display(Name = "Tip Text")]
        public string Text { get; set; }
    }
}