# DocSpace submission index

**Purpose:** lightweight authenticated document workspace assessment.

- Stack: .NET 8, Razor Pages, ASP.NET Core Identity, EF Core/SQLite, Quill, Open XML SDK, QuestPDF.
- Source: repository root.
- Tests: `dotnet test DocSpace.sln` — 3 passed in the latest audit run.
- Setup: see `README.md`.
- Sharing: register two accounts, create as account A, share with account B's registered email, then log in as B.
- Imports: TXT, Markdown, and constrained DOCX support are present in code.
- PDF/version history: implemented in code but not fully audited end to end.
- Deployment URL: pending; no verified URL.
- Walkthrough video: pending; no verified URL.

Known limitations include CDN Quill, legacy startup schema bridge instead of migrations, and incomplete DOCX/PDF fidelity/testing. With another 2–4 hours: add integration tests, verify browser flows, add PDF UI and formatting fidelity, and replace the schema bridge with reviewed migrations.
