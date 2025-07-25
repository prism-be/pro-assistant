using Microsoft.EntityFrameworkCore;
using Prism.ProAssistant.Domain.DayToDay.Contacts;

namespace Prism.Infrastructure.Providers.Azure;

public class ProAssistantDbContext(DbContextOptions<ProAssistantDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>()
            .HasKey(x => x.Id);
    }
}