using System;
using Microsoft.EntityFrameworkCore;
using App.Core;
using Microsoft.Extensions.Configuration;
using App.Domain;

namespace App.Database
{
    public class DataContext : DbContext
    {
        public DbSet<alienEntity> aliens { get; set; }
        public DbSet<sightingEntity> sightings { get; set; }

        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
                var configurationBuilder = new ConfigurationBuilder()
                                            .AddJsonFile("appsettings.json", optional: true, true)
                                            .AddJsonFile($"appsettings.{environment}.json", true, true)
                                            .AddEnvironmentVariables()
                                            .Build();

                options.UseMySql(
                    configurationBuilder["ConnectionStrings:DefaultConnection"],
                    ServerVersion.AutoDetect(configurationBuilder["ConnectionStrings:DefaultConnection"])
                    );
            }

        }   
    }
}