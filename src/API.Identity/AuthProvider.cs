using System.Security.Claims;
using JWTSimpleServer;
using JWTSimpleServer.Abstractions;
using Orleans.Tournament.API.Identity.Authentication;

namespace Orleans.Tournament.API.Identity;

public class AuthProvider : IAuthenticationProvider
{
    private readonly IUserStore _userStore;

    public AuthProvider(IUserStore userStore)
    {
        _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
    }

    public async Task ValidateClientAuthentication(JwtSimpleServerContext context)
    {
        (await _userStore.GetUserAsync(context.UserName, context.Password)).Match(
            s => context.Success(GetUserClaims(s)),
            f => context.Reject("Invalid user authentication"));
    }

    private List<Claim> GetUserClaims(User user)
    {
        return new[] {new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())}
            .Concat(user.Claims.Select(e => new Claim(ClaimTypes.Role, e))).ToList();
    }
}