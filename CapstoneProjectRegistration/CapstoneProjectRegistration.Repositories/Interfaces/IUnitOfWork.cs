using CapstoneProjectRegistration.Repositories.Entities;

namespace CapstoneProjectRegistration.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Admin> Admins { get; }

    IGenericRepository<Lecturer> Lecturers { get; }

    IGenericRepository<Student> Students { get; }

    IGenericRepository<Semester> Semesters { get; }

    IGenericRepository<Topic> Topics { get; }

    IGenericRepository<TopicReview> TopicReviews { get; }

    Task<int> SaveChangesAsync();
}
