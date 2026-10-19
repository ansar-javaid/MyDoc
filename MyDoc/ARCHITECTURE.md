# Architecture

DocSpace is one Razor Pages application. `DocSpaceDbContext` owns SQLite entities: `User`, `Document`, and `DocumentShare`; unique email and `(DocumentId, UserId)` indexes protect the essential constraints. Startup resolves an absolute database path, calls `EnsureCreated`, and seeds Alice and Bob once.

Pages use focused services: `DemoUserService` resolves the session identity from the database, `DocumentAuthorizationService` calculates Owner/Editor/Viewer/None, `DocumentImportService` converts a constrained upload, and `HtmlSafetyService` normalizes stored editor HTML. Every document handler checks access server-side; only owners may share or revoke.

The editor loads saved HTML into Quill. It debounces saves and retains unsaved browser content on failure, with an explicit Save action. Save sanitizes HTML and stores title/content/time in UTC. Markdown disables embedded HTML; plain text is HTML encoded. SQLite is chosen for low operational complexity and is appropriate only where a durable disk is available.

Scope cuts: no migrations (first-run `EnsureCreated`), no real auth, concurrency tokens, comments, history, or live collaboration. Next steps would be a vetted sanitizer, production identity, EF migrations, optimistic concurrency, and locally hosted Quill assets.
