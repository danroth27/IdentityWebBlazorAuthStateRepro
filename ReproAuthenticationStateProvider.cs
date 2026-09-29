using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace IdentityWebBlazorAuthStateRepro;

// Synthetic, circuit-scoped identities only; this is not an authentication implementation.
public sealed class ReproAuthenticationStateProvider : AuthenticationStateProvider
{
    private AuthenticationState _state = new(CreateUser("Alice"));

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(_state);

    public void SwitchToBob() => SetUser(CreateUser("Bob"));

    public void BecomeAnonymous() => SetUser(new ClaimsPrincipal(new ClaimsIdentity()));

    private void SetUser(ClaimsPrincipal user)
    {
        _state = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(_state));
    }

    private static ClaimsPrincipal CreateUser(string name) =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.Name, name)], "SyntheticRepro"));
}
