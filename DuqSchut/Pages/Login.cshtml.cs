#nullable disable
// Based on
// https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/authentication/cookie/samples/6.x/CookieSample/Pages/Account/Login.cshtml.cs
// which is referenced by
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-9.0 
using DuqSchut.Data;
using DuqSchut.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace DuqSchut.Pages
{
    public class LoginModel(
        ILogger<LoginModel> logger,
        IDbContextFactory<DuqSchutContext> dbContextFactory
        ) : PageModel
    {
        private readonly ILogger<LoginModel> _logger = logger;
        private readonly IDbContextFactory<DuqSchutContext> _dbContextFactory = dbContextFactory;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            public string Email { get; set; }

            // This will actually hold the user's name
            public string Password { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Clear the existing external cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Use Input.Email and Input.Password to authenticate the user
            // with your custom authentication logic.
            var userID = Input.Email;
            var userName = Input.Password;

            // TODO: If this user is not in the database, add them as a Student.
            await LoginUtils.AddUserToDatabaseIfNotExistsAsync(userID , userName , _dbContextFactory);
        
            // Use default role of Student unless a different role can be found in the DB
            var role = await LoginUtils.GetRoleAsync(userID, _dbContextFactory);

            // Create the claims for this user and log them in (by creating an HTTP 
            // cookie containing the user's credentials)
            var claims = LoginUtils.CreateClaims(userID, userName, role);
            var claimsIdentity = new ClaimsIdentity(
                claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);
            var user = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                user, 
                authProperties);
            _logger.LogInformation("User {Email} logged in at {Time}.", 
                Input.Email, DateTime.UtcNow);
           
            // Redirect request to the
            // correct landing page for this user's role.
            var redirectURL = LoginUtils.MapRoleToLandingURL(user);
            return LocalRedirect(redirectURL);
        }
    }
}