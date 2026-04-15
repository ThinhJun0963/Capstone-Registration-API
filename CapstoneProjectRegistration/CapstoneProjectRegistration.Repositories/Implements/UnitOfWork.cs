using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;

namespace CapstoneProjectRegistration.Repositories.Implements;

public class UnitOfWork : IUnitOfWork
{
    private readonly CapstoneDbContext _context;
    private IGenericRepository<Admin>? _admins;
    private IGenericRepository<Lecturer>? _lecturers;
    private IGenericRepository<Student>? _students;
    private IGenericRepository<Semester>? _semesters;
    private IGenericRepository<Topic>? _topics;
    private IGenericRepository<TopicReview>? _topicReviews;

    public UnitOfWork(CapstoneDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Admin> Admins =>
        _admins ??= new GenericRepository<Admin>(_context);

    public IGenericRepository<Lecturer> Lecturers =>
        _lecturers ??= new GenericRepository<Lecturer>(_context);

    public IGenericRepository<Student> Students =>
        _students ??= new GenericRepository<Student>(_context);

    public IGenericRepository<Semester> Semesters =>
        _semesters ??= new GenericRepository<Semester>(_context);

    public IGenericRepository<Topic> Topics =>
        _topics ??= new GenericRepository<Topic>(_context);

    public IGenericRepository<TopicReview> TopicReviews =>
        _topicReviews ??= new GenericRepository<TopicReview>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        // DbContext lifetime is managed by the DI container when registered as scoped.
    }
}
