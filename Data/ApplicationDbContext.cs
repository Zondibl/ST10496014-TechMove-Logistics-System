using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EAPD7111wPOE_Part1.Models;

namespace EAPD7111wPOE_Part1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<Contract> Contracts { get; set; }
    }
}
