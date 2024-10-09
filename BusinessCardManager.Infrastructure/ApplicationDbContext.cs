// ApplicationDbContext is the central database context for the Business Card Manager application.
// It manages the BusinessCard entities and configures their properties using Fluent API to define 
// database constraints such as primary keys, required fields, and maximum lengths. This class ensures 
// that the database schema aligns with the application's data requirements.


using BusinessCardManager.Core.Entities; 
using Microsoft.EntityFrameworkCore; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Reflection.Emit; 
using System.Text; 
using System.Threading.Tasks; 

namespace BusinessCardManager.Infrastructure 
{
    // Class representing the application's database context
    public class ApplicationDbContext : DbContext
    {
        // Constructor accepting DbContextOptions and passing it to the base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet representing the BusinessCards table in the database
        public DbSet<BusinessCard> BusinessCards { get; set; }

        // Method to configure the model and relationships using Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the BusinessCard entity
            modelBuilder.Entity<BusinessCard>(entity =>
            {
                entity.HasKey(b => b.Id); // Define Id as the Primary Key

                // Configure Name property
                entity.Property(b => b.Name)
                    .IsRequired() // Name is required
                    .HasMaxLength(100); // Maximum length of Name is 100 characters

                // Configure Gender property
                entity.Property(b => b.Gender)
                    .HasMaxLength(10); // Maximum length of Gender is 10 characters

                // Configure DateOfBirth property
                entity.Property(b => b.DateOfBirth)
                    .IsRequired(); // DateOfBirth is required

                // Configure Email property
                entity.Property(b => b.Email)
                    .IsRequired() // Email is required
                    .HasMaxLength(100); // Maximum length of Email is 100 characters

                // Configure Phone property
                entity.Property(b => b.Phone)
                    .HasMaxLength(15); // Maximum length of Phone is 15 characters

                // Configure Address property
                entity.Property(b => b.Address)
                    .HasMaxLength(255); // Maximum length of Address is 255 characters

                // Configure Photo property
                entity.Property(b => b.Photo)
                    .HasMaxLength(500); // Maximum length for Base64 encoded photo string
            });
        }
    }
}
