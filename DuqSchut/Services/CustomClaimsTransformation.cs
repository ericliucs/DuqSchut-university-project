/*
Suggested by ChatGPT in response to multiple prompts:
1. In a Blazor Web App using Entra ID for authentication, how can I add roles to the 
authentication state?
2. I want to do this using Entra ID only for authentication, with no roles assigned 
in Entra ID.  The roles will come from a database after authentication.
3. This approach produced a run-time error:
InvalidOperationException: A circular dependency was detected for the service of type 
'Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider'.
4. The first page that accesses that uses the AuthenticationStateProvider receives an 
AuthenticationState with the role set correctly to "Admin". However, this page navigates 
to a second page that contains an "@attribute [ Authorize(Roles = "Admin")]" statement.  
This second page returns an HTTP 403 Not Authorized status code. 
5. Now when I navigate to the login page this exception is thrown:|
Application: SnapshotUploader.exe Framework Version: v4.0.30319 Description: 
The process was terminated due to an unhandled exception. 
Exception Info: System.UnauthorizedAccessException 
Exception Info: System.IO.FileLoadException
*/

using DuqSchut.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace DuqSchut.Services
{
    /**
     <summary>
      Service class that adds a user's role to their authentication state shortly
      after authentication occurs and before any post-login Blazor pages are processed. 
      The class constructor accepts the injection of a DbContextFactory.
     </summary>
    */
    public class CustomClaimsTransformation
        (IDbContextFactory<DuqSchutContext> dbContextFactory) : IClaimsTransformation
    {
        private readonly IDbContextFactory<DuqSchutContext> _dbContextFactory 
            = dbContextFactory;

        /**
         <summary>
          Given a ClaimsPrincipal, return a new ClaimsPrincipal that contains all
          of the claims of the original plus a new claim for the user's role as
          defined in the database.  This new ClaimsPrincipal will be used in the
          remainder of the app to authorize the user. Note that this method is
          run on each AuthenticateAsync call per
          https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.iclaimstransformation?view=aspnetcore-9.0
         </summary>
         <param name="principal">The original ClaimsPrincipal constructed by Entra
         ID software based on the response from Multipass.</param>
         <returns>The new ClaimsPrincipal containing all of the original claims
         along with a role claim, or the original ClaimsPrincipal if the user is
         not authenticated or already has a role in the original
         ClaimsPrincipal.</returns>
        */
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // If input principal already has a role, or if the user is not
            // authenticated, return the principal unchanged.
            // See https://learn.microsoft.com/en-us/aspnet/core/security/authentication/claims?view=aspnetcore-9.0#extend-or-add-custom-claims-using-iclaimstransformation
            if (principal.HasClaim(claim => claim.Type == ClaimTypes.Role)
                || principal.Identity is not { IsAuthenticated: true })
            {
                return principal;
            }

            // Attempt to retrieve this user's role from the database. This defaults
            // to Student if the user's email address is not in the database.
            var userId = principal.FindFirstValue("email");
            var role = await LoginUtils.GetRoleAsync(userId, _dbContextFactory);

            // Attempting to add a claim to the original principal's Identity
            // failed with a System.UnauthorizedAccessException. So, instead,
            // create and return a new principal for this user that includes 
            // all of the original claims plus a claim for the user's role.
            var claims = new List<Claim>(principal.Claims)
            {
                new(ClaimTypes.Role, role)
            };
            var newIdentity = new ClaimsIdentity(
                claims, principal.Identity.AuthenticationType);
            return new ClaimsPrincipal(newIdentity);
        }
    }
}