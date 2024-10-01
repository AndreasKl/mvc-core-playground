using System.Security.Claims;
using HelloMVCCore.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HelloMVCCore;

public class TestClaimsTransformation(UserManager<ApplicationUser> userManager) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var claimsIdentity = new ClaimsIdentity();


        var user = await userManager.GetUserAsync(principal);
        if(user == null)
        {
            return principal;
        }
        
        if (!principal.HasClaim(claim => claim.Type == "language"))
        {
            claimsIdentity.AddClaim(new Claim("language", user.Language));
        }

        principal.AddIdentity(claimsIdentity);
        return principal;
    }
}