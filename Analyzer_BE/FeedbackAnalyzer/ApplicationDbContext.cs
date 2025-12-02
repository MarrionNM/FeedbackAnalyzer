using FeedbackAnalyzer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace FeedbackAnalyzer;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Feedback> Feedback { get; set; }
    public DbSet<FeedbackAnalysis> FeedbackAnalysis { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<FeedbackTag> FeedbackTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- JSON converter for embeddings ---
        var floatArrayConverter = new ValueConverter<float[], string>(
            to => JsonConvert.SerializeObject(to),
            from => JsonConvert.DeserializeObject<float[]>(from)
        );

        modelBuilder.Entity<Feedback>()
            .Property(f => f.VectorEmbedding)
            .HasConversion(floatArrayConverter)
            .HasColumnType("jsonb"); // For SQLite, this becomes TEXT

        modelBuilder.Entity<FeedbackAnalysis>()
            .HasKey(fa => fa.Id);

        modelBuilder.Entity<FeedbackAnalysis>()
            .HasIndex(fa => fa.FeedbackId)
            .IsUnique();

        modelBuilder.Entity<FeedbackAnalysis>()
            .HasOne(fa => fa.Feedback)
            .WithOne(f => f.Analysis)
            .HasForeignKey<FeedbackAnalysis>(fa => fa.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade);

        // many-to-many mapping
        modelBuilder.Entity<FeedbackTag>()
            .HasKey(ft => new { ft.FeedbackId, ft.TagId });

        modelBuilder.Entity<FeedbackTag>()
            .HasOne(ft => ft.Feedback)
            .WithMany(f => f.FeedbackTags)
            .HasForeignKey(ft => ft.FeedbackId);

        modelBuilder.Entity<FeedbackTag>()
            .HasOne(ft => ft.Tag)
            .WithMany(t => t.FeedbackTags)
            .HasForeignKey(ft => ft.TagId);

        // make Tag.Name unique
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyEntityRules();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyEntityRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyEntityRules()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Base && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified ||
                e.State == EntityState.Deleted));

        foreach (var entry in entries)
        {
            var entity = (Base)entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.Id = string.IsNullOrEmpty(entity.Id)
                        ? Guid.NewGuid().ToString()
                        : entity.Id;

                    entity.CreatedDate = DateTime.UtcNow;
                    entity.UpdatedDate = DateTime.UtcNow;
                    entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entity.UpdatedDate = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = DateTime.UtcNow;
                    entity.IsActive = false;
                    break;
            }
        }
    }
}
