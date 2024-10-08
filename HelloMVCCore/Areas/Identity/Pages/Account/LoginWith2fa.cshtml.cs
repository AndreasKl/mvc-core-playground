#nullable disable

using System.ComponentModel.DataAnnotations;
using HelloMVCCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelloMVCCore.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginWith2faModel> logger
    ) : PageModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(
                7,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6
            )]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(
                    $"Unable to load two-factor authentication user."
                );
            }

            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(
                    $"Unable to load two-factor authentication user."
                );
            }

            var authenticatorCode = Input
                .TwoFactorCode.Replace(" ", string.Empty)
                .Replace("-", string.Empty);

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
                authenticatorCode,
                rememberMe,
                Input.RememberMachine
            );

            var userId = await userManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                // Reset selected thing on login
                var currentUser = await userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    currentUser.CurrentThingRole = null;
                    currentUser.CurrentThingID = null;
                    await userManager.UpdateAsync(currentUser);
                }

                logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            logger.LogWarning(
                "Invalid authenticator code entered for user with ID '{UserId}'.",
                user.Id
            );
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return Page();
        }
    }
}
