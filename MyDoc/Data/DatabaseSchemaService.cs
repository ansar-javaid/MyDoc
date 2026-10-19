using Microsoft.EntityFrameworkCore;

namespace MyDoc.Data;
// Bridges the original Phase 1 schema in place. Existing integer user IDs stay unchanged.
public static class DatabaseSchemaService
{
 public static async Task PrepareAsync(DocSpaceDbContext db,CancellationToken ct=default)
 {
  await db.Database.EnsureCreatedAsync(ct);
  var columns=await ColumnsAsync(db,"Users",ct);
  foreach(var (name,type,definition) in new[]{("AccessFailedCount","INTEGER","INTEGER NOT NULL DEFAULT 0"),("ConcurrencyStamp","TEXT","TEXT NULL"),("EmailConfirmed","INTEGER","INTEGER NOT NULL DEFAULT 0"),("LockoutEnabled","INTEGER","INTEGER NOT NULL DEFAULT 0"),("LockoutEnd","TEXT","TEXT NULL"),("NormalizedEmail","TEXT","TEXT NULL"),("NormalizedUserName","TEXT","TEXT NULL"),("PasswordHash","TEXT","TEXT NULL"),("PhoneNumber","TEXT","TEXT NULL"),("PhoneNumberConfirmed","INTEGER","INTEGER NOT NULL DEFAULT 0"),("SecurityStamp","TEXT","TEXT NULL"),("TwoFactorEnabled","INTEGER","INTEGER NOT NULL DEFAULT 0"),("UserName","TEXT","TEXT NULL")})if(!columns.Contains(name))await db.Database.ExecuteSqlRawAsync($"ALTER TABLE Users ADD COLUMN {name} {definition}",ct);
  await db.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS UserNameIndex ON Users (NormalizedUserName)",ct);
  await db.Database.ExecuteSqlRawAsync("CREATE INDEX IF NOT EXISTS EmailIndex ON Users (NormalizedEmail)",ct);
  await db.Database.ExecuteSqlRawAsync("UPDATE Users SET UserName=Email, NormalizedEmail=UPPER(Email), NormalizedUserName=UPPER(Email), ConcurrencyStamp=COALESCE(ConcurrencyStamp, lower(hex(randomblob(16)))), SecurityStamp=COALESCE(SecurityStamp, lower(hex(randomblob(16)))) WHERE UserName IS NULL OR NormalizedEmail IS NULL OR NormalizedUserName IS NULL",ct);
  foreach(var sql in IdentityTables)await db.Database.ExecuteSqlRawAsync(sql,ct);
  await db.Database.ExecuteSqlRawAsync("CREATE TABLE IF NOT EXISTS DocumentVersions (Id INTEGER NOT NULL CONSTRAINT PK_DocumentVersions PRIMARY KEY AUTOINCREMENT, DocumentId INTEGER NOT NULL, VersionNumber INTEGER NOT NULL, Title TEXT NOT NULL, ContentHtml TEXT NOT NULL, CreatedByUserId INTEGER NOT NULL, CreatedAtUtc TEXT NOT NULL, CONSTRAINT FK_DocumentVersions_Documents_DocumentId FOREIGN KEY(DocumentId) REFERENCES Documents(Id) ON DELETE CASCADE, CONSTRAINT FK_DocumentVersions_Users_CreatedByUserId FOREIGN KEY(CreatedByUserId) REFERENCES Users(Id) ON DELETE RESTRICT)",ct);
  await db.Database.ExecuteSqlRawAsync("CREATE UNIQUE INDEX IF NOT EXISTS IX_DocumentVersions_DocumentId_VersionNumber ON DocumentVersions(DocumentId,VersionNumber)",ct);
 }
 static readonly string[] IdentityTables=[
 "CREATE TABLE IF NOT EXISTS AspNetRoles (Id INTEGER NOT NULL CONSTRAINT PK_AspNetRoles PRIMARY KEY AUTOINCREMENT, Name TEXT NULL, NormalizedName TEXT NULL, ConcurrencyStamp TEXT NULL)",
 "CREATE UNIQUE INDEX IF NOT EXISTS RoleNameIndex ON AspNetRoles(NormalizedName)",
 "CREATE TABLE IF NOT EXISTS AspNetUserClaims (Id INTEGER NOT NULL CONSTRAINT PK_AspNetUserClaims PRIMARY KEY AUTOINCREMENT, UserId INTEGER NOT NULL, ClaimType TEXT NULL, ClaimValue TEXT NULL, CONSTRAINT FK_AspNetUserClaims_Users_UserId FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE)",
 "CREATE INDEX IF NOT EXISTS IX_AspNetUserClaims_UserId ON AspNetUserClaims(UserId)",
 "CREATE TABLE IF NOT EXISTS AspNetUserLogins (LoginProvider TEXT NOT NULL, ProviderKey TEXT NOT NULL, ProviderDisplayName TEXT NULL, UserId INTEGER NOT NULL, CONSTRAINT PK_AspNetUserLogins PRIMARY KEY(LoginProvider,ProviderKey), CONSTRAINT FK_AspNetUserLogins_Users_UserId FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE)",
 "CREATE INDEX IF NOT EXISTS IX_AspNetUserLogins_UserId ON AspNetUserLogins(UserId)",
 "CREATE TABLE IF NOT EXISTS AspNetUserRoles (UserId INTEGER NOT NULL, RoleId INTEGER NOT NULL, CONSTRAINT PK_AspNetUserRoles PRIMARY KEY(UserId,RoleId), CONSTRAINT FK_AspNetUserRoles_Users_UserId FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE, CONSTRAINT FK_AspNetUserRoles_Roles_RoleId FOREIGN KEY(RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE)",
 "CREATE INDEX IF NOT EXISTS IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId)",
 "CREATE TABLE IF NOT EXISTS AspNetUserTokens (UserId INTEGER NOT NULL, LoginProvider TEXT NOT NULL, Name TEXT NOT NULL, Value TEXT NULL, CONSTRAINT PK_AspNetUserTokens PRIMARY KEY(UserId,LoginProvider,Name), CONSTRAINT FK_AspNetUserTokens_Users_UserId FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE)" ];
 static async Task<HashSet<string>> ColumnsAsync(DocSpaceDbContext db,string table,CancellationToken ct){var result=new HashSet<string>(StringComparer.OrdinalIgnoreCase);await using var command=db.Database.GetDbConnection().CreateCommand();command.CommandText=$"PRAGMA table_info({table})";if(command.Connection!.State!=System.Data.ConnectionState.Open)await command.Connection.OpenAsync(ct);await using var reader=await command.ExecuteReaderAsync(ct);while(await reader.ReadAsync(ct))result.Add(reader.GetString(1));return result;}
}
