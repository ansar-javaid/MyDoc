using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace MyDoc.Models;
public sealed class ApplicationUser:IdentityUser<int>{[Required,StringLength(100)]public string FullName{get;set;}=string.Empty;public ICollection<Document> OwnedDocuments{get;set;}=new List<Document>();}
public sealed class Document{public int Id{get;set;}[Required,StringLength(200)]public string Title{get;set;}="Untitled document";[Required]public string ContentHtml{get;set;}="<p><br></p>";public int OwnerId{get;set;}public ApplicationUser Owner{get;set;}=null!;public DateTime CreatedAtUtc{get;set;}public DateTime UpdatedAtUtc{get;set;}public ICollection<DocumentShare> Shares{get;set;}=new List<DocumentShare>();public ICollection<DocumentVersion> Versions{get;set;}=new List<DocumentVersion>();}
public enum SharePermission{Viewer,Editor}
public sealed class DocumentShare{public int Id{get;set;}public int DocumentId{get;set;}public Document Document{get;set;}=null!;public int UserId{get;set;}public ApplicationUser User{get;set;}=null!;public SharePermission Permission{get;set;}}
public sealed class DocumentVersion{public int Id{get;set;}public int DocumentId{get;set;}public Document Document{get;set;}=null!;public int VersionNumber{get;set;}[Required,StringLength(200)]public string Title{get;set;}=string.Empty;[Required]public string ContentHtml{get;set;}=string.Empty;public int CreatedByUserId{get;set;}public ApplicationUser CreatedByUser{get;set;}=null!;public DateTime CreatedAtUtc{get;set;}}
