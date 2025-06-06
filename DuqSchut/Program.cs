using DuqSchut.Components;
using Microsoft.EntityFrameworkCore;
using DuqSchut.Data;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using DuqSchut.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging.AzureAppServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<DuqSchutContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DuqSchutContext") ?? throw new InvalidOperationException("Connection string 'DuqSchutContext' not found.")));

// Add Razor services to the container.
// Options suggested by 
// https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/handle-errors?view=aspnetcore-9.0#detailed-errors-for-razor-component-server-side-rendering
builder.Services.AddRazorComponents(options => 
    options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents();
builder.Services.AddQuickGridEntityFrameworkAdapter();

//add the Email service and the interface. 
builder.Services.AddSingleton<IEmailDependency, EmailService>();
builder.Services.AddSingleton<EmailService>();

// add the Appointment service 
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
// add the Report service and the interface
builder.Services.AddSingleton<IReportDependency, ReportService>();

// Allow for use of [CascadingParameter] Task<AuthenticationState> ... to access
// authentication state.
builder.Services.AddCascadingAuthenticationState();

// Azure logger. Per the following, only runs when in an Azure environment.
// https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers#azure-app-service
builder.Logging.AddAzureWebAppDiagnostics();
// Following can be used to override defaults of 10MB file size and 2 retained files.
/*
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "azure-diagnostics-";
    options.FileSizeLimit = 50 * 1024; // 50 KB
    options.RetainedFileCountLimit = 5;
});
*/
builder.Services.Configure<AzureBlobLoggerOptions>(options =>
{
    options.IsEnabled = false;
});

// Add additional services based on the environment, development or otherwise.
if (builder.Environment.IsDevelopment()) {
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddRazorPages(); // Login is a Razor page
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login";
                });
}
else 
{
    // Entra ID identity services for Multipass authentication from an Azure server.
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

    builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

    // Services to add role to the authentication state. Thanks, ChatGPT!
    builder.Services.AddAuthorization(); // not sure that this is needed
    builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseDeveloperExceptionPage();
    app.MapRazorPages();
    app.MapDefaultControllerRoute();
    // app.UseMigrationsEndPoint();
}
else if (app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}
else // Production
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

// Currently deleting and rebuilding database when it needs to be revised
// rather than using database migrations to move to new versions.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DuqSchutContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();


