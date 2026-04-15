using CapstoneProjectRegistration.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProjectRegistration.Services.Respond.Topics
{
    public class TopicRespond
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string EnglishName { get; set; } = string.Empty;

        [StringLength(255)]
        public string VietnameseName { get; set; } = string.Empty;

        public int SemesterId { get; set; }


        public int CreatorId { get; set; }


        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
