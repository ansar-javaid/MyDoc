using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyDoc.Data;
using MyDoc.Models;
using MyDoc.Services;

namespace MyDoc.Pages.Documents;
public sealed class EditModel(DocSpaceDbContext db, IDemoUserService users, IDocumentAuthorizationService auth, IHtmlSafetyService safety) : PageModel
{
 public Document? Document { get; private set; } public User? CurrentUser { get; private set; } public DocumentAccess Access { get; private set; } public List<User> OtherUsers { get; private set; }=[];
 public bool CanEdit => Access is DocumentAccess.Owner or DocumentAccess.Editor; public bool IsOwner => Access == DocumentAccess.Owner;
 public async Task<IActionResult> OnGetAsync(int id,CancellationToken ct){if(!await LoadAsync(id,ct))return NotFound();return Page();}
 async Task<bool> LoadAsync(int id,CancellationToken ct){CurrentUser=await users.GetCurrentAsync(HttpContext,ct);if(CurrentUser is null)return false;Access=await auth.GetAccessAsync(id,CurrentUser.Id,ct);if(Access==DocumentAccess.None)return false;Document=await db.Documents.Include(d=>d.Owner).Include(d=>d.Shares).ThenInclude(s=>s.User).FirstOrDefaultAsync(d=>d.Id==id,ct);OtherUsers=await db.Users.Where(u=>u.Id!=CurrentUser.Id).OrderBy(u=>u.Name).ToListAsync(ct);return Document is not null;}
 public async Task<IActionResult> OnPostSaveAsync(int id,[FromForm]string title,[FromForm]string contentHtml,CancellationToken ct){var user=await users.GetCurrentAsync(HttpContext,ct);if(user is null)return Unauthorized();var access=await auth.GetAccessAsync(id,user.Id,ct);if(access is not(DocumentAccess.Owner or DocumentAccess.Editor))return Forbid();if(string.IsNullOrWhiteSpace(title)||title.Trim().Length>200)return BadRequest(new{error="A title of 1–200 characters is required."});var doc=await db.Documents.FindAsync([id],ct);if(doc is null)return NotFound();doc.Title=title.Trim();doc.ContentHtml=safety.Sanitize(contentHtml);doc.UpdatedAtUtc=DateTime.UtcNow;await db.SaveChangesAsync(ct);return new JsonResult(new{savedAt=doc.UpdatedAtUtc});}
 public async Task<IActionResult> OnPostShareAsync(int id,int userId,SharePermission permission,CancellationToken ct){var user=await users.GetCurrentAsync(HttpContext,ct);if(user is null)return Unauthorized();var access=await auth.GetAccessAsync(id,user.Id,ct);if(access!=DocumentAccess.Owner)return Forbid();if(userId==user.Id||!await db.Users.AnyAsync(u=>u.Id==userId,ct)){TempData["Error"]="Choose another valid demo user.";return RedirectToPage(new{id});}var share=await db.DocumentShares.FirstOrDefaultAsync(s=>s.DocumentId==id&&s.UserId==userId,ct);if(share is null)db.DocumentShares.Add(new DocumentShare{DocumentId=id,UserId=userId,Permission=permission});else share.Permission=permission;await db.SaveChangesAsync(ct);TempData["Success"]="Sharing permission saved.";return RedirectToPage(new{id});}
 public async Task<IActionResult> OnPostRevokeAsync(int id,int userId,CancellationToken ct){var user=await users.GetCurrentAsync(HttpContext,ct);if(user is null)return Unauthorized();if(await auth.GetAccessAsync(id,user.Id,ct)!=DocumentAccess.Owner)return Forbid();var share=await db.DocumentShares.FirstOrDefaultAsync(s=>s.DocumentId==id&&s.UserId==userId,ct);if(share is not null){db.DocumentShares.Remove(share);await db.SaveChangesAsync(ct);TempData["Success"]="Access revoked.";}return RedirectToPage(new{id});}
}
