using System.Security.Claims;
using HelloMVCCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HelloMVCCore;

public class MyCustomClaimsFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser>(userManager, optionsAccessor) 
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var baseClaims = await base.GenerateClaimsAsync(user);
        
        baseClaims.AddClaim(new Claim("thing", "23"));
        baseClaims.AddClaim(new Claim("thing|23", "123"));
        baseClaims.AddClaim(new Claim("thing|23", "Guest"));
        
        return baseClaims;
    }
}