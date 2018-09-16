using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JWTSimpleServer;
using JWTSimpleServer.Abstractions;

namespace Snaelro.API.Identity
{
    public class AuthProvider : IAuthenticationProvider
    {
        public Task ValidateClientAuthentication(JwtSimpleServerContext context)
        {
            if(context.UserName == "demo" && context.Password == "demo")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "level_one"),
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                };

                context.Success(claims);
            }
            else
            {
                context.Reject("Invalid user authentication");
            }

            return Task.CompletedTask;
        }
    }
}