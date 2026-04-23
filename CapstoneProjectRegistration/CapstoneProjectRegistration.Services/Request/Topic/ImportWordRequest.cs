using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProjectRegistration.Services.Request.Topic;

public class ImportWordRequest
{
    [FromForm(Name = "file")]
    public IFormFile? File { get; set; }
}
