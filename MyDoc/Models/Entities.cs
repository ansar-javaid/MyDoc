using System.ComponentModel.DataAnnotations;

namespace MyDoc.Models;

public sealed class User
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(254)] public string Email { get; set; } = string.Empty;
    public ICollection<Document> OwnedDocuments { get; set; } = new List<Document>();
}

public sealed class Document
{
    public int Id { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = "Untitled document";
    [Required] public string ContentHtml { get; set; } = "<p><br></p>";
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
}

public enum SharePermission { Viewer, Editor }

public sealed class DocumentShare
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public SharePermission Permission { get; set; }
}
