using Microsoft.AspNetCore.Identity;

namespace HelloMVCCore.Data;

public class ApplicationUser : IdentityUser
{
    public string Language { get; set; }
}
