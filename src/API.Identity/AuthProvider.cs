using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JWTSimpleServer;
using JWTSimpleServer.Abstractions;
using Orleans.Tournament.API.Identity.Authentication;

namespace Orleans.Tournament.API.Identity
{
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

        public List<Claim> GetUserClaims(User user)
            => new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }
                .Concat(user.Claims.Select(e => new Claim(ClaimTypes.Role, e))).ToList();
    }
}
