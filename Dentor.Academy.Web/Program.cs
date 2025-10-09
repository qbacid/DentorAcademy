using Dentor.Academy.Web.Components;
using Dentor.Academy.Web.Data;
using Dentor.Academy.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure PostgreSQL Database Context
builder.Services.AddDbContext<QuizDbContext>(options =>
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
        .UseSnakeCaseNamingConvention() // PostgreSQL convention
);

// Register Quiz Services
builder.Services.AddScoped<QuizScoringService>();
builder.Services.AddScoped<QuizImportService>();
builder.Services.AddScoped<QuizTakingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();