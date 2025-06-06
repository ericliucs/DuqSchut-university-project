namespace DuqSchut.Tests.Services;

using System.Security.Claims;
using System.Threading.Tasks;
using DuqSchut.Data;
using DuqSchut.Models;
using DuqSchut.Services;
using Microsoft.EntityFrameworkCore;

/**
 <summary>
  xUnit tests for the CustomClaimsTransformation service.
 </summary>
*/
public class CustomClaimsTransformationTests
{
    private readonly IDbContextFactory<DuqSchutContext> factory;
    /**
     <summary>
      Create separate empty database for each test.
     </summary>
    */
    public CustomClaimsTransformationTests()
    {
        var dbName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddDbContextFactory<DuqSchutContext>(
            options => options.UseInMemoryDatabase(databaseName: dbName)
        );
        var serviceProvider = services.BuildServiceProvider();
        factory = serviceProvider.GetRequiredService<IDbContextFactory<DuqSchutContext>>();
    }

    [Fact]
    public async Task NoClaimsAddedIfUserHasRole()
    {
        // Arrange
        var sut = new CustomClaimsTransformation(factory);
        var role = UserRole.Admin.ToString();
        var claims = new List<Claim>()
        {
            new(ClaimTypes.Role, role)
        };
        // Helped by https://stackoverflow.com/questions/17425854/how-to-create-a-claimsprincipal-that-has-identity-authenticated-set-to-true
        var identity = new ClaimsIdentity(claims, "Basic");
        var principal = new ClaimsPrincipal(identity);

        // Act: Call TransformAsync with principal that has single role claim
        var newPrincipal = await sut.TransformAsync(principal);
                
        // Assert: Returned principal has only one claim which is the original role
        Assert.Single(newPrincipal.Claims);
        Assert.True(newPrincipal.IsInRole(role));
    }

    [Fact]
    public async Task NoClaimsAddedIfUserIsNotAuthenticated()
    {
        // Arrange
        var sut = new CustomClaimsTransformation(factory);
        var principal = new ClaimsPrincipal();

        // Act: Call TransformAsync with unauthenticated principal
        var newPrincipal = await sut.TransformAsync(principal);

        // Assert: No claims included in returned principal
        Assert.Empty(newPrincipal.Claims);
    }

    [Fact]
    public async Task NonExistingUserHasStudentRoleAddedToAuthenticationState()
    {
        // Arrange
        var sut = new CustomClaimsTransformation(factory);
        // Helped by https://stackoverflow.com/questions/17425854/how-to-create-a-claimsprincipal-that-has-identity-authenticated-set-to-true
        var identity = new ClaimsIdentity(null, "Basic");
        var principal = new ClaimsPrincipal(identity);

        // Act: Call TransformAsync with an empty but authenticated principal
        var newPrincipal = await sut.TransformAsync(principal);

        // Assert: Returned principal contains Student role
        Assert.True(newPrincipal.HasClaim(claim => claim.Type == ClaimTypes.Role));
        Assert.True(newPrincipal.IsInRole(UserRole.Student.ToString()));
    }
}
