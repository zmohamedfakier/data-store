using DataStore.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStore.Services.Source
{
  public class SourceDbContext : DbContext
  {
    public DbSet<SourceDescriptor> Source { get; set; }

    public SourceDbContext(DbContextOptions options)
      :base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      //optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=SamuraiAppData;User Id=postgres;Password=sparklebutterfly1");
      base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      EntityTypeBuilder<SourceDescriptor> sourceModel =  modelBuilder.Entity<SourceDescriptor>();

      sourceModel.ToTable("Source", "Meta").HasKey(s => s.ID);

      sourceModel.Property(s => s.ID).HasColumnName("ID");
      sourceModel.Property(s => s.Name).HasColumnName("Name");

      base.OnModelCreating(modelBuilder);
    }
  }
}
