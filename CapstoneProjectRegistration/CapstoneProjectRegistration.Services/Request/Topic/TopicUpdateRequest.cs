using CapstoneProjectRegistration.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProjectRegistration.Services.Request.Topic
{
    public class TopicUpdateRequest
    {
        public string EnglishName { get; set; } = string.Empty;

        [StringLength(255)]
        public string VietnameseName { get; set; } = string.Empty;

        public int SemesterId { get; set; }

    }
}
