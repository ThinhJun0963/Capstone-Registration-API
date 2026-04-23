using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProjectRegistration.Services.Request.AccountRequest
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Student | Lecturer | Admin

        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
