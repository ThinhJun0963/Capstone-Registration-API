using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;

namespace CapstoneProjectRegistration.Repositories.Implements;

public class LectureRepository : GenericRepository<Lecturer>, ILectureRepository
{
    public LectureRepository(CapstoneDbContext context) : base(context)
    {
        
    }
}
