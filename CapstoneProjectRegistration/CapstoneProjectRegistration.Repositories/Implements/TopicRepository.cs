using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;

namespace CapstoneProjectRegistration.Repositories.Implements
{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(CapstoneDbContext context) : base(context)
        {
        }
    }
}
