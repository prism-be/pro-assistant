using Microsoft.EntityFrameworkCore;
using Prism.Infrastructure.Authentication;
using Prism.ProAssistant.Domain.DayToDay.Contacts;

namespace Prism.Temp.Migrator;

public class ProAssistantDbContext: DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<UserOrganization> UserOrganizations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQLDB_CONNECTION_STRING"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<UserOrganization>()
            .HasKey(x => x.Id);
    }
}