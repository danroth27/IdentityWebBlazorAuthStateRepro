# IdentityWebBlazorAuthStateRepro

| Setting | Value |
|---------|-------|
| **Interactivity Mode** | Server |
| **Interactivity Scope** | Per-page |

## Rendering configuration

Created with `dotnet new blazor --empty -int Server -f net10.0`.

Pages are static SSR by default. The home page uses `InteractiveServerRenderMode(prerender: false)` because the consent handler's user is initialized when the circuit opens, not during prerendering.

## Components and services

- Keep component logic inline in `.razor` files.
- Components inject server-side services directly; no HTTP API is needed.
- Keep the home page's authentication state cascading so Blazor handles provider notifications.
- Do not make `Routes` globally interactive.
- Do not read `HttpContext.User` to represent the circuit's current user.

## Reproduction constraints

- Use the published Microsoft.Identity.Web package and its public `AddMicrosoftIdentityConsentHandler()` registration.
- Do not copy, replace, or patch the library's internal circuit handler.
- The custom authentication-state provider is synthetic and circuit-scoped. Do not add real credentials, Entra registrations, cookies, or tokens.
- Preserve the existing bug rather than implementing its fix in this repository.
- Keep this a small local diagnostic app, not a production authentication sample.
