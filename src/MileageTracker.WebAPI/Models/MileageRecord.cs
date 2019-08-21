using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MileageTracker.WebAPI.Models
{
    [Table("MileageRecord")]
    public class MileageRecord
    {
        [Key]
        public int MileageRecordId { get; set; }
        [Display(Name = "Service")]
        public string Service { get; set; }
        [Display(Name = "Mileage")]
        public double Mileage { get; set; }
        [Display(Name = "Start DateTime")]
        public DateTime StartDateTime { get; set; }
        [Display(Name = "End DateTime")]
        public DateTime EndDateTime { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}