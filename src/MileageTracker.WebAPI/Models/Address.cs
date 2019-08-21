using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MileageTracker.WebAPI.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Display(Name = "Address Line One")]
        public string AddressLineOne { get; set; }
        [Display(Name = "Address Line Two")]
        public string AddressLineTwo { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "State Abbreviation")]
        public string StateAbbreviation { get; set; }
        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}