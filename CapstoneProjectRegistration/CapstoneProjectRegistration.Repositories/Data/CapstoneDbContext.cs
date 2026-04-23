using CapstoneProjectRegistration.Repositories.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CapstoneProjectRegistration.Repositories.Data;

public class CapstoneDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admins => Set<Admin>();

    public DbSet<Lecturer> Lecturers => Set<Lecturer>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Semester> Semesters => Set<Semester>();

    public DbSet<RegistrationPeriod> RegistrationPeriods => Set<RegistrationPeriod>();

    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<TopicReview> TopicReviews => Set<TopicReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasOne(a => a.ApplicationUser)
                .WithMany()
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasIndex(l => l.ApplicationUserId).IsUnique().HasFilter("[ApplicationUserId] IS NOT NULL");

            entity.HasOne(l => l.ApplicationUser)
                .WithMany()
                .HasForeignKey(l => l.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(s => s.ApplicationUserId).IsUnique().HasFilter("[ApplicationUserId] IS NOT NULL");

            entity.HasOne(s => s.ApplicationUser)
                .WithMany()
                .HasForeignKey(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasIndex(t => t.TopicCode).IsUnique();

            entity.HasOne(t => t.Semester)
                .WithMany(s => s.Topics)
                .HasForeignKey(t => t.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.RegistrationPeriod)
                .WithMany(rp => rp.Topics)
                .HasForeignKey(t => t.RegistrationPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Creator)
                .WithMany(l => l.Topics)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RegistrationPeriod>(entity =>
        {
            entity.HasOne(rp => rp.Semester)
                .WithMany(s => s.RegistrationPeriods)
                .HasForeignKey(rp => rp.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TopicReview>(entity =>
        {
            entity.HasIndex(tr => new { tr.TopicId, tr.ReviewerId }).IsUnique();

            entity.HasOne(tr => tr.Topic)
                .WithMany(t => t.TopicReviews)
                .HasForeignKey(tr => tr.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tr => tr.Reviewer)
                .WithMany(l => l.TopicReviews)
                .HasForeignKey(tr => tr.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Semester>().HasData(
            new Semester
            {
                Id = 1,
                Name = "Spring 2026",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 5, 31),
                Status = "Active"
            });

        modelBuilder.Entity<RegistrationPeriod>().HasData(
            new RegistrationPeriod
            {
                Id = 1,
                Name = "Spring 2026 - Week 1",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 4, 7),
                Status = "Active",
                SemesterId = 1
            },
            new RegistrationPeriod
            {
                Id = 2,
                Name = "Spring 2026 - Week 2",
                StartDate = new DateTime(2026, 4, 8),
                EndDate = new DateTime(2026, 4, 14),
                Status = "Inactive",
                SemesterId = 1
            },
            new RegistrationPeriod
            {
                Id = 3,
                Name = "Spring 2026 - Week 3",
                StartDate = new DateTime(2026, 4, 15),
                EndDate = new DateTime(2026, 4, 21),
                Status = "Inactive",
                SemesterId = 1
            },
            new RegistrationPeriod
            {
                Id = 4,
                Name = "Spring 2026 - Week 4",
                StartDate = new DateTime(2026, 4, 22),
                EndDate = new DateTime(2026, 4, 28),
                Status = "Inactive",
                SemesterId = 1
            },
            new RegistrationPeriod
            {
                Id = 5,
                Name = "Spring 2026 - Final intake",
                StartDate = new DateTime(2026, 4, 29),
                EndDate = new DateTime(2026, 5, 5),
                Status = "Inactive",
                SemesterId = 1
            });

        modelBuilder.Entity<Lecturer>().HasData(
            new Lecturer
            {
                Id = 1,
                Name = "Lecturer 1",
                Email = "lecturer1@gmail.com",
                Phone = "0900000001",
                Title = "Dr",
                Specialization = "Software Engineering",
                Status = "Active"
            },
            new Lecturer
            {
                Id = 2,
                Name = "Lecturer 2",
                Email = "lecturer2@gmail.com",
                Phone = "0900000002",
                Title = "Ms",
                Specialization = "Information Systems",
                Status = "Active"
            },
            new Lecturer
            {
                Id = 3,
                Name = "Lecturer 3",
                Email = "lecturer3@gmail.com",
                Phone = "0900000003",
                Title = "Mr",
                Specialization = "AI",
                Status = "Active"
            },
            new Lecturer
            {
                Id = 4,
                Name = "Lecturer 4",
                Email = "lecturer4@gmail.com",
                Phone = "0900000004",
                Title = "Prof",
                Specialization = "Data Science",
                Status = "Active"
            });

        modelBuilder.Entity<Admin>().HasData(
            new Admin
            {
                Id = 1,
                Name = "System Admin",
                Email = "admin@gmail.com",
                Status = "Active"
            });

        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                Id = 1,
                Name = "Pham Dinh Quoc Thinh",
                Email = "thinhpdqse171589@fpt.edu.vn",
                StudentCode = "SE171589",
                GroupRole = "Leader",
                Phone = "0842918005",
                Status = "Active"
            },
            new Student
            {
                Id = 2,
                Name = "Nguyen Nhat Nam",
                Email = "namnnse182539@fpt.edu.vn",
                StudentCode = "SE182539",
                GroupRole = "Member",
                Phone = "0704656071",
                Status = "Active"
            });

        modelBuilder.Entity<Topic>().HasData(
            new Topic
            {
                Id = 1001,
                EnglishName = "BoneVisQA Interactive Learning",
                VietnameseName = "BoneVisQA Hỏi Đáp Trực Quan",
                TopicCode = "SU26SE0011",
                Description = "Learning system for bone diseases.",
                SemesterId = 1,
                RegistrationPeriodId = 1,
                CreatorId = 1,
                Status = "Approved",
                ReviewStatus = "Approved",
                PublicStatus = "Public",
                CreatedAt = new DateTime(2026, 4, 3, 8, 0, 0)
            },
            new Topic
            {
                Id = 1002,
                EnglishName = "Capstone Topic Similarity Scanner",
                VietnameseName = "Hệ thống số khớp đề tài do an",
                TopicCode = "SU26SE002",
                Description = "Detect duplicated project ideas via fuzzy matching and semantic scoring.",
                SemesterId = 1,
                RegistrationPeriodId = 1,
                CreatorId = 4,
                Status = "Rejected",
                ReviewStatus = "Rejected",
                PublicStatus = "Private",
                CreatedAt = new DateTime(2026, 4, 4, 9, 0, 0)
            },
            new Topic
            {
                Id = 1003,
                EnglishName = "Campus Room Booking Assistant",
                VietnameseName = "Trợ lý đặt phòng học trong campus",
                TopicCode = "SU26SE003",
                Description = "Mobile-first booking flow with conflict detection and notifications.",
                SemesterId = 1,
                RegistrationPeriodId = 2,
                CreatorId = 2,
                Status = "Pending",
                ReviewStatus = "Pending",
                PublicStatus = "Private",
                CreatedAt = new DateTime(2026, 4, 10, 10, 0, 0)
            },
            new Topic
            {
                Id = 1004,
                EnglishName = "Graduation Checklist Tracker",
                VietnameseName = "Theo dõi checklist tốt nghiệp",
                TopicCode = "SU26SE004",
                Description = "Dashboard for students and staff to track graduation requirements.",
                SemesterId = 1,
                RegistrationPeriodId = 2,
                CreatorId = 3,
                Status = "Approved",
                ReviewStatus = "Approved",
                PublicStatus = "Public",
                CreatedAt = new DateTime(2026, 4, 11, 14, 30, 0)
            },
            new Topic
            {
                Id = 1005,
                EnglishName = "Peer Code Review Metrics",
                VietnameseName = "Đo lường peer code review",
                TopicCode = "SU26SE005",
                Description = "Collect Git-based metrics and visualize review quality over sprints.",
                SemesterId = 1,
                RegistrationPeriodId = 3,
                CreatorId = 1,
                Status = "Draft",
                ReviewStatus = "Pending",
                PublicStatus = "Private",
                CreatedAt = new DateTime(2026, 4, 16, 9, 15, 0)
            });

        modelBuilder.Entity<TopicReview>().HasData(
            new TopicReview
            {
                Id = 5001,
                TopicId = 1001,
                ReviewerId = 2,
                Decision = "Approved",
                Comment = "Clear scope.",
                ReviewDate = new DateTime(2026, 4, 6, 10, 0, 0),
                IsFinalized = true
            },
            new TopicReview
            {
                Id = 5002,
                TopicId = 1001,
                ReviewerId = 3,
                Decision = "Approved",
                Comment = "Good practical value.",
                ReviewDate = new DateTime(2026, 4, 6, 11, 0, 0),
                IsFinalized = true
            },
            new TopicReview
            {
                Id = 5003,
                TopicId = 1002,
                ReviewerId = 2,
                Decision = "Rejected",
                Comment = "Topic overlaps too much with existing works.",
                ReviewDate = new DateTime(2026, 4, 7, 10, 0, 0),
                IsFinalized = true
            },
            new TopicReview
            {
                Id = 5004,
                TopicId = 1002,
                ReviewerId = 3,
                Decision = "Approved",
                Comment = "Acceptable but revise objectives.",
                ReviewDate = new DateTime(2026, 4, 7, 11, 0, 0),
                IsFinalized = true
            });
    }
}
