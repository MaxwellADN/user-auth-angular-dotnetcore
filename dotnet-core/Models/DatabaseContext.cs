using System;
using Microsoft.EntityFrameworkCore;

namespace dotnet_core.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(){}

        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.EnableSensitiveDataLogging();

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Create default roles with migration
            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = 1,
                    Name = "Admin",
                    CreatedAt = DateTime.Now
                },
                new Role()
                {
                    Id = 2,
                    Name = "User",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}