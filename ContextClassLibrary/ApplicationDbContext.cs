using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ContextClassLibrary
{
    // Design Time Factory for CLI "dotnet ef"
    public class CliDbContext : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("PGSQL"),
                options => options
                    .UseNodaTime()
                    .EnableRetryOnFailure(3)
                    .CommandTimeout(30)
                    .SetPostgresVersion(new Version(11, 2))
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

    public class ApplicationDbContext : DbContext
    {
        private IConfigurationRoot _configuration;
        private string _environment;

        // constructor for mocking with InMemory provider
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // return if already configured (by mocking with InMemory provider)
            if (optionsBuilder.IsConfigured)
                return;

            ConfigureConfiguration();

            optionsBuilder
                .UseNpgsql(
                    _configuration.GetConnectionString("PGSQL"),
                    npgsql => npgsql
                        .UseNodaTime()
                        .EnableRetryOnFailure(3)
                        .CommandTimeout(30)
                        .SetPostgresVersion(new Version(11, 2))
                );

            if (_environment == "Production") return;

            // Enable Sensitive data logging and detailed errors outside Production environment
            optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();
        }

        private void ConfigureConfiguration()
        {
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            Console.WriteLine("Environment: {0}", _environment);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{_environment}.json", optional: true);

            _configuration = builder.Build();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Table2>()
                .HasOne(p => p.Table1)
                .WithOne(p => p.Table2);
        }

        public DbSet<Table1> Table1s { get; set; }
        public DbSet<Table2> Table2s { get; set; }
    }
}