using Microsoft.EntityFrameworkCore;
using ResumeRadar.Api.Models;

namespace ResumeRadar.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Analysis> Analyses => Set<Analysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.GitHubId).IsUnique();
            e.Property(u => u.ResumeText).HasColumnType("TEXT");
        });

        modelBuilder.Entity<Analysis>(e =>
        {
            e.HasOne(a => a.User)
             .WithMany(u => u.Analyses)
             .HasForeignKey(a => a.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
