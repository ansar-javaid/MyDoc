using Microsoft.EntityFrameworkCore;
using MyDoc.Data;
using MyDoc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.Cookie.Name = ".DocSpace.Session"; options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; options.IdleTimeout = TimeSpan.FromHours(8); });
var dbPath = builder.Configuration["DocSpace:DatabasePath"] ?? Path.Combine(builder.Environment.ContentRootPath, "data", "docspace.db");
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);
builder.Services.AddDbContext<DocSpaceDbContext>(o => o.UseSqlite($"Data Source={Path.GetFullPath(dbPath)}"));
builder.Services.AddScoped<IDocumentAuthorizationService, DocumentAuthorizationService>();
builder.Services.AddScoped<IDemoUserService, DemoUserService>();
builder.Services.AddSingleton<IHtmlSafetyService, HtmlSafetyService>();
builder.Services.AddScoped<IDocumentImportService, DocumentImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope()) await DatabaseSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<DocSpaceDbContext>());

app.Run();
