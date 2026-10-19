# DocSpace

DocSpace is a lightweight document editor assessment built as one ASP.NET Core Razor Pages application. It supports persistent rich-text documents, safe TXT/Markdown import, sharing, and server-enforced viewer/editor access.

## Stack and features

- .NET 8 Razor Pages, Bootstrap, Quill 2 (official CDN assets), EF Core 8 and SQLite
- Create, rename (in editor), save/reopen, delete, format, import, share/revoke, and demo-user switching
- Quill HTML is sanitized before storage; Markdown has raw HTML disabled before conversion

## Run locally

```powershell
dotnet restore DocSpace.sln
dotnet run --project MyDoc.csproj
```

Open the HTTPS URL printed by ASP.NET Core. On first launch the app creates and seeds `data/docspace.db` with Alice (`alice@docspace.local`) and Bob (`bob@docspace.local`). Select either user in the header; the selection is validated and stored in a server-managed session. To use another absolute database file, set `DocSpace__DatabasePath`, for example `$env:DocSpace__DatabasePath='C:\data\docspace.db'`.

## Test

```powershell
dotnet test DocSpace.sln
```

Tests use isolated in-memory SQLite connections, never the application database.

## Import and configuration

Only UTF-8 `.txt` and `.md` files up to 1 MB are accepted. Quill is loaded from jsDelivr, so editor formatting requires internet access (the rest of the app remains available). SQLite uses `EnsureCreated` for reliable first-run initialization rather than migrations.

## Word import and PDF export

Authenticated users can import `.docx` files up to 5 MB from the dashboard. The server verifies the ZIP signature and expected Word package entry, limits ZIP entry count/uncompressed size, and uses Open XML SDK to preserve ordinary paragraphs, headings, bold, italic, and underline. Complex layouts, images, tables, tracked changes, macros, and external resources are intentionally ignored. Accessible saved documents can be downloaded from `/documents/{id}/pdf`; PDF output uses QuestPDF Community edition, needs no native renderer or external service, and currently renders a safe readable text representation of the stored rich text.

## Deployment

Publish with `dotnet publish -c Release`. Deploy only to a host with a persistent mounted disk and set `DocSpace__DatabasePath` to a location on that disk. A free host with ephemeral storage is not suitable for durable SQLite. No deployment has been performed because it requires an account and persistent-volume choice.

## Security and limitations

Demo identity is deliberately not production authentication; it is a server-side session selected from seeded users. Anti-forgery protects posts and authorization is checked in handlers. The compact sanitizer is deliberately restrictive but is not a substitute for a mature production HTML sanitization policy. No real-time collaboration, comments, version history, or account management is included.
