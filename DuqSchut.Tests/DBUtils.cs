namespace DuqSchut.Tests;

using System;
using DuqSchut.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Moq;

/**
 <summary>
  Class to contain database-related utility methods in support of tests.
 </summary>
*/
public class DBUtils 
{
    /** 
    <summary>
     Create and register a database factory for the test
     calling this method.  The factory will create an
     in-memory database that is unique to this test. The
     factory is registered for injection into the component
     under test. For convenience, this method also registers
     QuickGrid, which is used by many auto-generated Blazor pages, 
     although this should probably be moved to its own method. 
    </summary>
    <param name="testContext">The object calling this method,
      which is a subclass of TestContext. This provides access
      to bUnit test objects such as Services.</param>
    */



    public static void CreateDB(TestContext testContext)
    {
        // Create and register a DbContextFactory that will generate
        // an in-memory database with a unique name for testing purposes.  
        var dbName = Guid.NewGuid().ToString();
        testContext.Services.AddDbContextFactory<DuqSchutContext>(


                options => options.UseInMemoryDatabase(databaseName: dbName)
        );

        // Also register QuickGrid support for rendering Blazor pages.
        testContext.Services.AddQuickGridEntityFrameworkAdapter();
        testContext.JSInterop.Mode = JSRuntimeMode.Loose; // To avoid unsupported method exceptions
        
        // Fake the environment as Development
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(m => m.EnvironmentName).Returns("Development");
        testContext.Services.AddSingleton<IWebHostEnvironment>(mockEnv.Object);
    }
}