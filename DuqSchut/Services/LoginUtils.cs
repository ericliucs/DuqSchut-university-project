namespace DuqSchut.Services;

using System.Security.Claims;
using System.Threading.Tasks;
using DuqSchut.Data;
using DuqSchut.Models;
using Microsoft.EntityFrameworkCore;



/**
 <summary>
  Utility methods to aid in login. Can be used for both Azure/Multipass and 
  local development login.
 </summary>
*/
public class LoginUtils {

    /**
     <summary>
      Retrieve the role for the specified user by consulting the database.
      Returns Student role if user is not in database or has no role assigned.
     </summary>
     <param name="userID">ID (email address) of the user.</param>
     <param name="dbContextFactory">Context factory for accessing database.</param>
    */
    public static async Task<string> GetRoleAsync(
        string userID, 
        IDbContextFactory<DuqSchutContext> dbContextFactory) {

        // Default to Student role
        var role = UserRole.Student.ToString();

        // If user is in database, return their Role, if they have one
        using var dbContext = dbContextFactory.CreateDbContext();
        var user = await dbContext.Users.FindAsync(userID);
        if (user?.Role is not null) {
            role = user.Role.ToString();
        }
        return role;
    }

    /**
     <summary>
      Create an array of Claim objects for this user given the user's id (email address),
      name, and role.
     </summary>
     <param name="email">User's email address; goes in claim "email".</param>
     <param name="name">User's name; goes in claim "name".</param>
     <param name="role">User's role; goes in the <c>ClaimTypes.Role</c> claim.</param>
    */
    public static Claim[] CreateClaims(string email, string name, string role) {
        return
        [
            new Claim("email", email),
            new Claim("name", name),
            new Claim(ClaimTypes.Role, role),
        ];
    }

    /**
     <summary>
      Given a <c>ClaimsPrincipal</c>, which might be null, return a URL to the
      landing page for users in the given role.  Return the Error page URL
      if the <c>ClaimsPrincipal</c> is null or the role is not one that we
      recognize.
     </summary>
     <param name="user"><c>ClaimsPrincipal</c> of the user.</param>
    */
    public static string MapRoleToLandingURL(ClaimsPrincipal user) {
        var landingURL = "";
        if (user is null) {
            landingURL = "/Error";
        }
        else if (user.IsInRole("Student")) {
            landingURL = "/student/appointments";
        }
        else if (user.IsInRole("Admin")) {
            landingURL = "/admin/NavPage";
        }
        else if (user.IsInRole("Tutor")) {
            landingURL = "/tutor/tutorhome";
        }
        else {
            landingURL = "/Error";
        }
        return landingURL;
    }
    /**
     <summary>
      If the user is not already in the database while logging in, we add it.
     </summary>
     <param name="userID">User's email address. Used as primary key</param>
     <param name="userName">User's full name.</param>
     <param name="dbContextFactory">allows us to get a context of the db <c>IDbContextFactory</c></param>
    */
    public static async Task AddUserToDatabaseIfNotExistsAsync(string userID , string userName, IDbContextFactory<DuqSchutContext> dbContextFactory){
        using var context = await dbContextFactory.CreateDbContextAsync();
            var userExists = context.Users.Any(e => e.ID == userID);
            if(!userExists){
                var firstname = "";
                var lastname = "";
                int index = userName.IndexOf(' ');
                if(index == -1){
                    firstname = userName;
                }
                else{
                    firstname = userName.Substring(0, index);
                    lastname = userName.Substring(index + 1);
                }
                

                var newUser = new User{
                    ID = userID,
                    FirstName = firstname,
                    LastName = lastname,
                    Role = UserRole.Student
                };
                context.Users.Add(newUser);
                await context.SaveChangesAsync();

        }
    }
}