using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyDoc.Data;
using MyDoc.Models;
using MyDoc.Services;
namespace MyDoc.Pages;
public sealed class IndexModel(DocSpaceDbContext db, IDemoUserService users, IDocumentImportService importer) : PageModel
{
 public User? CurrentUser { get; private set; } public List<Document> Owned { get; private set; }=[]; public List<Document> Shared { get; private set; }=[];
 [BindProperty] public IFormFile? ImportFile { get; set; } [BindProperty] public string? ImportTitle { get; set; }
 public async Task OnGetAsync(CancellationToken ct) { CurrentUser=await users.GetCurrentAsync(HttpContext,ct); if(CurrentUser is null)return; Owned=await db.Documents.Include(d=>d.Owner).Where(d=>d.OwnerId==CurrentUser.Id).OrderByDescending(d=>d.UpdatedAtUtc).ToListAsync(ct); Shared=await db.Documents.Include(d=>d.Owner).Include(d=>d.Shares).Where(d=>d.OwnerId!=CurrentUser.Id&&d.Shares.Any(s=>s.UserId==CurrentUser.Id)).OrderByDescending(d=>d.UpdatedAtUtc).ToListAsync(ct); }
 public async Task<IActionResult> OnPostSwitchUserAsync(int userId,CancellationToken ct) { if(!await db.Users.AnyAsync(u=>u.Id==userId,ct)){TempData["Error"]="That demo user is not available.";return RedirectToPage();} HttpContext.Session.SetInt32(DemoUserService.SessionKey,userId);TempData["Success"]="Demo identity switched.";return RedirectToPage(); }
 public async Task<IActionResult> OnPostCreateAsync(CancellationToken ct) { var user=await users.GetCurrentAsync(HttpContext,ct);if(user is null)return Unauthorized();var doc=new Document{OwnerId=user.Id,Title="Untitled document",ContentHtml="<p><br></p>",CreatedAtUtc=DateTime.UtcNow,UpdatedAtUtc=DateTime.UtcNow};db.Documents.Add(doc);await db.SaveChangesAsync(ct);return RedirectToPage("/Documents/Edit",new{id=doc.Id}); }
 public async Task<IActionResult> OnPostImportAsync(CancellationToken ct) { var user=await users.GetCurrentAsync(HttpContext,ct);if(user is null)return Unauthorized();var result=await importer.ConvertAsync(ImportFile,ct);if(!result.Ok){TempData["Error"]=result.Error;return RedirectToPage();}var name=string.IsNullOrWhiteSpace(ImportTitle)?Path.GetFileNameWithoutExtension(ImportFile!.FileName):ImportTitle.Trim();if(name.Length>200)name=name[..200];var doc=new Document{OwnerId=user.Id,Title=name,ContentHtml=result.Html,CreatedAtUtc=DateTime.UtcNow,UpdatedAtUtc=DateTime.UtcNow};db.Documents.Add(doc);await db.SaveChangesAsync(ct);TempData["Success"]="File imported into a new document.";return RedirectToPage("/Documents/Edit",new{id=doc.Id}); }
 public async Task<IActionResult> OnPostDeleteAsync(int id,CancellationToken ct) { var user=await users.GetCurrentAsync(HttpContext,ct);if(user is null)return Unauthorized();var doc=await db.Documents.FirstOrDefaultAsync(d=>d.Id==id&&d.OwnerId==user.Id,ct);if(doc is null)return NotFound();db.Documents.Remove(doc);await db.SaveChangesAsync(ct);TempData["Success"]="Document deleted.";return RedirectToPage(); }
}
