using Microsoft.EntityFrameworkCore;
using MyDoc.Data;
using MyDoc.Models;

namespace MyDoc.Services;
public interface IDemoUserService { Task<User?> GetCurrentAsync(HttpContext context, CancellationToken ct = default); }
public sealed class DemoUserService(DocSpaceDbContext db) : IDemoUserService
{
    public const string SessionKey = "DocSpace.DemoUserId";
    public async Task<User?> GetCurrentAsync(HttpContext context, CancellationToken ct = default)
    {
        var id = context.Session.GetInt32(SessionKey);
        return id is null ? null : await db.Users.FindAsync([id.Value], ct);
    }
}
