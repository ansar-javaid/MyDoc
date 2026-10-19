# 3–5 minute walkthrough script

1. Introduce DocSpace: a .NET 8 Razor Pages, EF Core/SQLite document workspace.
2. Select Alice, create a document, change its title, format text with Quill, save, refresh, and reopen it.
3. Import a small TXT or Markdown file and show it opened as a new editable document.
4. From Alice's sharing panel, grant Bob Viewer access. Switch to Bob, show the shared section and read-only editor. Attempting a server save as a viewer is forbidden.
5. Switch back to Alice, change Bob to Editor; switch to Bob and save an edit. Mention that owners alone can share/revoke.
6. Explain session demo identity, centralized authorization, sanitized rich text, and persistent SQLite. Show `dotnet test DocSpace.sln` output.
7. State actual AI usage honestly using `AI_WORKFLOW.md`, then note simulated auth, CDN dependency, no history/real-time collaboration, and planned production improvements.
