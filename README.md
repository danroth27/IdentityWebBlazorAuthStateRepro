# Microsoft.Identity.Web Blazor authentication-state repro

A minimal .NET 10 Blazor Server app demonstrating that `MicrosoftIdentityConsentAndConditionalAccessHandler.User` retains the circuit's initial principal after `AuthenticationStateProvider.AuthenticationStateChanged` is raised.

The app references **Microsoft.Identity.Web 4.15.0 from NuGet**. It uses the package's real circuit handler through `AddMicrosoftIdentityConsentHandler()`; no library internals are copied, mocked, or modified.

## Run

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then run from the repository directory:

```console
dotnet run --launch-profile http
```

Open http://localhost:5011.

`global.json` selects an installed, released .NET 10 SDK and permits later .NET 10 feature bands. No .NET 11 SDK is required.

The app uses loopback HTTP and synthetic identities. No Entra tenant, app registration, credentials, development certificate, or Microsoft.Identity.Web source checkout is required.

## Reproduce

1. On initial load, both table rows show **Alice**, authenticated.
2. Click **Switch to Bob** without reloading. Blazor's current user changes to **Bob**, but the consent handler still shows **Alice**. The page reports the mismatch.
3. Click **Become anonymous**. Blazor's current user becomes **(anonymous)** with `Authenticated = False`, while the consent handler still shows **Alice** with `Authenticated = True`.
4. Click **Start a new circuit** to reload. Both rows start as Alice again.

| Action | Blazor authentication state | Consent handler user (actual) | Consent handler user (expected) |
|--------|-----------------------------|-------------------------------|--------------------------------|
| Initial load | Alice | Alice | Alice |
| Switch to Bob | Bob | Alice | Bob |
| Become anonymous | (anonymous) | Alice | (anonymous) |

These buttons do not perform real sign-in or sign-out. They synchronously publish completed authentication-state tasks through the provider, isolating the missing subscription from networking, token acquisition, and asynchronous ordering.

The stale principal can affect user information used by incremental-consent and conditional-access challenges. This repro does not demonstrate an authorization bypass or perform a challenge against an identity provider.

## Relevant code

- [`Program.cs`](Program.cs) registers the same scoped provider for the page and Blazor, cascading authentication state, and the package's consent handler.
- [`ReproAuthenticationStateProvider.cs`](ReproAuthenticationStateProvider.cs) starts with Alice and publishes changes using `NotifyAuthenticationStateChanged`.
- [`Components/Pages/Home.razor`](Components/Pages/Home.razor) displays the cascading authentication state beside `ConsentHandler.User`.

Prerendering is disabled on the repro page so both values are read within the actual Blazor circuit, after the package's circuit-open initialization.

This tracks the existing stale-state behavior discussed during [AzureAD/microsoft-identity-web#4068](https://github.com/AzureAD/microsoft-identity-web/pull/4068). It does not reproduce the separate initialization race in that PR's unmerged implementation.

This is a diagnostic reproduction only. Do not use its synthetic authentication provider in a production application.
