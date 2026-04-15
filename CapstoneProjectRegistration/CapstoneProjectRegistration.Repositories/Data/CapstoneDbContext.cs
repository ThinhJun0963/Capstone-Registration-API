using CapstoneProjectRegistration.Repositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectRegistration.Repositories.Data;

public class CapstoneDbContext : DbContext
{
    public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins => Set<Admin>();

    public DbSet<Lecturer> Lecturers => Set<Lecturer>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Semester> Semesters => Set<Semester>();

    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<TopicReview> TopicReviews => Set<TopicReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasOne(t => t.Semester)
                .WithMany(s => s.Topics)
                .HasForeignKey(t => t.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Creator)
                .WithMany(l => l.Topics)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TopicReview>(entity =>
        {
            entity.HasOne(tr => tr.Topic)
                .WithMany(t => t.TopicReviews)
                .HasForeignKey(tr => tr.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tr => tr.Reviewer)
                .WithMany(l => l.TopicReviews)
                .HasForeignKey(tr => tr.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
