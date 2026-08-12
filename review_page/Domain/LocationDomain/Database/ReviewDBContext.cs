using review_page.Domain.LocationDomain.Core.Data;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
namespace review_page.Domain.LocationDomain.Database
{
    public class ReviewDBContext : DbContext
    {
        public ReviewDBContext(DbContextOptions<ReviewDBContext> options) : base(options) { }

        public DbSet<Review> ReviewDB { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

        }


    }
}
