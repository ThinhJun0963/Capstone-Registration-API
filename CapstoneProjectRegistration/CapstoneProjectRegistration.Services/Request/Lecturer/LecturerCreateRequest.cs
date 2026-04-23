using System.ComponentModel.DataAnnotations;

namespace CapstoneProjectRegistration.Services.Request.Lecturer
{
    public class LecturerCreateRequest
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20)]
        public string Title { get; set; } = "Mr";

        [StringLength(255)]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Active";
    }
}
