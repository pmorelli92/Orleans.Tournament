using System.Collections.Immutable;

namespace Orleans.Tournament.API.Identity.Authentication;

public class User
{
    public User(Guid id, IImmutableList<string> claims)
    {
        Id = id;
        Claims = claims;
    }

    public Guid Id { get; }

    public IImmutableList<string> Claims { get; }
}