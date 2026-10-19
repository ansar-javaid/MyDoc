# Architecture

`DocSpaceDbContext` combines Identity's integer-key user store with `Document`, `DocumentShare`, and `DocumentVersion`. Existing document ownership remains keyed to the same user IDs. Razor Page handlers resolve the authenticated server principal through `CurrentUserService`; `DocumentAuthorizationService` returns Owner, Editor, Viewer, or None and handlers enforce it rather than trusting browser-supplied owners.

`DocumentImportService` handles TXT/Markdown. `DocxImportService` validates a constrained Open Packaging ZIP, reads it using Open XML SDK, converts a small formatting subset to HTML, then passes it through `HtmlSafetyService` (HtmlSanitizer allowlist). `PdfExportService` sanitizes saved content and produces a QuestPDF document; the PDF route checks access first.

Saves sanitize content and create historical snapshots when a document meaningfully changes. The SQLite database path is configurable through `DocSpace__DatabasePath`; startup's `DatabaseSchemaService` preserves the original table and adds required Identity/version columns and tables in place. This is a pragmatic legacy-upgrade bridge, not a replacement for reviewed EF migrations.
