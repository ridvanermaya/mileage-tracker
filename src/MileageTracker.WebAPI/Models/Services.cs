using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MileageTracker.WebAPI.Models
{
    [Table("Service")]
    public class Services
    {
        [Key]
        public int ServiceId { get; set; } 
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}