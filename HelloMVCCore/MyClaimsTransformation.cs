using System.Security.Claims;
using HelloMVCCore.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HelloMVCCore;

public class TestClaimsTransformation(UserManager<ApplicationUser> userManager)
    : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            return principal;
        }

        var claimsIdentity = new ClaimsIdentity();
        AddClaimIfNotPresent(principal, claimsIdentity, "language", user.Language);
        AddClaimIfNotPresent(
            principal,
            claimsIdentity,
            "currentThingID",
            user.CurrentThingID?.ToString()
        );
        AddClaimIfNotPresent(principal, claimsIdentity, "currentThingRole", user.CurrentThingRole);

        principal.AddIdentity(claimsIdentity);
        return principal;
    }

    private static void AddClaimIfNotPresent(
        ClaimsPrincipal currentPrincipal,
        ClaimsIdentity newClaimsIdentity,
        string claimType,
        string? value
    )
    {
        if (value == null)
            return;
        if (!currentPrincipal.HasClaim(claim => claim.Type == claimType))
        {
            newClaimsIdentity.AddClaim(new Claim(claimType, value ?? string.Empty));
        }
    }
}
