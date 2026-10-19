using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyDoc.Data;
using MyDoc.Models;
using MyDoc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
var dbPath = builder.Configuration["DocSpace:DatabasePath"] ?? Path.Combine(builder.Environment.ContentRootPath, "data", "docspace.db");
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);
builder.Services.AddDbContext<DocSpaceDbContext>(o => o.UseSqlite($"Data Source={Path.GetFullPath(dbPath)}"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(o => { o.Password.RequiredLength=8;o.Password.RequireDigit=true;o.Password.RequireUppercase=true;o.Password.RequireNonAlphanumeric=true;o.User.RequireUniqueEmail=true;o.SignIn.RequireConfirmedAccount=false; }).AddEntityFrameworkStores<DocSpaceDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o=>{o.LoginPath="/Account/Login";o.AccessDeniedPath="/Account/AccessDenied";o.Cookie.Name=".DocSpace.Auth";o.Cookie.HttpOnly=true;o.Cookie.SameSite=SameSiteMode.Lax;o.SlidingExpiration=true;});
builder.Services.AddScoped<IDocumentAuthorizationService, DocumentAuthorizationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IHtmlSafetyService, HtmlSafetyService>();
builder.Services.AddScoped<IDocumentImportService, DocumentImportService>();
builder.Services.AddScoped<IDocxImportService, DocxImportService>();
builder.Services.AddScoped<IPdfExportService, PdfExportService>();

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/documents/{id:int}/pdf", async (int id, HttpContext context, ICurrentUserService current, IDocumentAuthorizationService authorization, DocSpaceDbContext db, IPdfExportService pdf) =>
{
    var user = await current.GetCurrentAsync(context.User); if (user is null) return Results.Challenge();
    if (await authorization.GetAccessAsync(id, user.Id) == DocumentAccess.None) return Results.NotFound();
    var document = await db.Documents.FindAsync(id); if (document is null) return Results.NotFound();
    var bytes = pdf.Create(document.Title, document.ContentHtml);
    var safe = string.Concat(document.Title.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '-' : c));
    return Results.File(bytes, "application/pdf", safe + ".pdf");
}).RequireAuthorization();

using (var scope = app.Services.CreateScope()) { var db=scope.ServiceProvider.GetRequiredService<DocSpaceDbContext>(); await DatabaseSchemaService.PrepareAsync(db); await DatabaseSeeder.SeedAsync(db,scope.ServiceProvider); }

app.Run();
