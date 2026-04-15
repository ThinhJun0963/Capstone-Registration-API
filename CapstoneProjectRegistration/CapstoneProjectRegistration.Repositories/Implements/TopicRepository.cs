using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProjectRegistration.Repositories.Implements

{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(CapstoneDbContext context) : base(context)
        {
        }
    }
}
