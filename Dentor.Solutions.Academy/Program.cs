using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Dentor.Solutions.Academy.Components;
using Dentor.Solutions.Academy.Components.Account;
using Dentor.Solutions.Academy.Data;
using Dentor.Solutions.Academy.Interfaces;
using Dentor.Solutions.Academy.Services;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

builder.Services.AddCascadingAuthenticationState();

// Configure PostgreSql Database Context
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseNpgsql(
                builder.Configuration.GetConnectionString("QuizDatabase"),
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                })
);

builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "DentorAcademy.Auth";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

//configure admin access
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrInstructorOnly", policy => policy.RequireRole("Admin", "Instructor"));
    options.AddPolicy("AdminOrStudentOnly", policy => policy.RequireRole("Admin", "Student"));
    options.AddPolicy("AdminOrCourseManagerOnly", policy => policy.RequireRole("Course Manager"));
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        // Simple password policy
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Configure file upload limits (Microsoft best practice)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB max for form data
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Register Services with Dependency Injection (using interfaces)
builder.Services.AddScoped<IQuizScoringService, QuizScoringService>();
builder.Services.AddScoped<IQuizImportService, QuizImportService>();
builder.Services.AddScoped<IQuizTakingService, QuizTakingService>();
builder.Services.AddScoped<IQuizManagementService, QuizManagementService>();
builder.Services.AddScoped<IUserPerformanceService, UserPerformanceService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
builder.Services.AddScoped<ICourseManagementService, CourseManagementService>();
builder.Services.AddScoped<ICourseContentService, CourseContentService>();

// Register Integration Services
builder.Services.AddHttpClient<IVimeoService, VimeoService>();
builder.Services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();

// Add Controllers for API endpoints (image serving)
builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

// Apply migrations and seed Admin role and user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    //await db.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Ensure all roles exist
    string[] roles = { "Admin", "Student", "Instructor", "Authenticated User", "Course Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = builder.Configuration["Admin:Email"];
    var adminPassword = builder.Configuration["Admin:Password"];

    if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
    {
        throw new InvalidOperationException("Admin credentials must be configured in appsettings.json");
    }


    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();