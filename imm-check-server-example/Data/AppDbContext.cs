using Microsoft.EntityFrameworkCore;
using imm_check_server_example.Models;

namespace imm_check_server_example.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ImmDocI20> I20Programs { get; set; } = null!;
    public DbSet<ImmDocDS2019info> DS2019Infos { get; set; } = null!;
    public DbSet<ImmDocI94> I94s { get; set; } = null!;
    public DbSet<ImmDocPassport> Passports { get; set; } = null!;
    public DbSet<ImmDocVisaInfo> VisaInfos { get; set; } = null!;
    public DbSet<SponsoredStudentInformation> SponsoredStudentInformation { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImmDocI20>(entity =>
        {
            entity.ToTable("I20Program");
            entity.HasKey(e => e.recnum);
        });

        modelBuilder.Entity<ImmDocDS2019info>(entity =>
        {
            entity.ToTable("ds2019info");
            entity.HasKey(e => e.recnum);
        });

        modelBuilder.Entity<ImmDocI94>(entity =>
        {
            entity.ToTable("I94");
            entity.HasKey(e => e.recnum);
        });

        modelBuilder.Entity<ImmDocPassport>(entity =>
        {
            entity.ToTable("passport");
            entity.HasKey(e => e.recnum);
        });

        modelBuilder.Entity<ImmDocVisaInfo>(entity =>
        {
            entity.ToTable("visaInfo");
            entity.HasKey(e => e.recnum);
        });

        modelBuilder.Entity<SponsoredStudentInformation>(entity =>
        {
            entity.ToTable("sponsoredStudentInformation");
            entity.HasKey(e => e.recnum);
        });
    }
}
