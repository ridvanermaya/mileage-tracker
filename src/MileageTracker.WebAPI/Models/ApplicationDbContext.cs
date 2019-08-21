using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MileageTracker.WebAPI.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Address> Addresses { get; set;}
        public DbSet<MileageRecord> MileageRecords { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}