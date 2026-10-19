# DocSpace

DocSpace is a .NET 8 Razor Pages document workspace with SQLite persistence, ASP.NET Core Identity, Quill editing, sharing, imports, version snapshots, and an authorized PDF endpoint.

## Setup

```powershell
dotnet restore DocSpace.sln
dotnet build DocSpace.sln
dotnet run --project MyDoc.csproj
dotnet test DocSpace.sln
```

The default database is `data/docspace.db`; set `DocSpace__DatabasePath` to an absolute durable path when deploying. Startup applies the repository's in-place schema preparation for the pre-existing SQLite layout and Identity tables. Back up the database before upgrading an existing installation.

## Accounts and sharing

Register at `/Account/Register`, then log in at `/Account/Login`. To test sharing, register two accounts, create a document as the first, share it by the second account's exact registered email, then log in as that account. Viewers can read; editors can save; only owners manage shares.

## Imports, exports, and limits

TXT and Markdown import is implemented in `DocumentImportService`; DOCX import accepts `.docx` packages up to 5 MB and validates ZIP signature, package structure, entry count, and expanded size. The DOCX converter supports ordinary text, headings, bold, italic, and underline. It deliberately ignores macros, images, tables, tracked changes, external content, and advanced layout. All imported/editor HTML is sanitized on the server.

The authorized endpoint `/documents/{id}/pdf` generates a PDF for an owner or collaborator. It uses QuestPDF Community edition without a native renderer or external service. The current implementation exports a safe text rendering rather than full HTML fidelity.

## Known limitations

Quill loads from jsDelivr. There are no EF migration files; the legacy database is updated by a guarded startup schema bridge. DOCX/PDF workflows require additional automated and browser verification before a production claim. No deployment URL or walkthrough video has been verified.
