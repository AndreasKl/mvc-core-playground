using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HelloMVCCore.Data;

public class ApplicationUser : IdentityUser
{
    public string? Language { get; set; }

    public int? CurrentThingID { get; set; }

    [MaxLength(200)]
    public string? CurrentThingRole { get; set; }
}
