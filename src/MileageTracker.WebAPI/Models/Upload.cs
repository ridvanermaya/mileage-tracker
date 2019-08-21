using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MileageTracker.WebAPI.Models
{
    [Table("Upload")]
    public class Upload
    {
        [Key]
        public int UploadId { get; set; }
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
        [Display(Name = "Upload DateTime")]
        public DateTime UploadDate { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}