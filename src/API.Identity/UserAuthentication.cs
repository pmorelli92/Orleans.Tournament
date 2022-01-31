using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Orleans.Tournament.API.Identity;

public record Login(string Email, string Password);

public record CreateUser(string Email, string Password, IList<string> Claims);

public interface ICreateUser
{
    Task<Guid> Handle(CreateUser request);
}

public interface ILoginUser
{
    Task<string> Handle(Login request);
}

public class UserAuthentication : ICreateUser, ILoginUser
{
    private readonly ConnectionString _connectionString;
    private readonly JwtConfiguration _jwtConfiguration;

    public UserAuthentication(ConnectionString connectionString, JwtConfiguration jwtConfiguration)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _jwtConfiguration = jwtConfiguration ?? throw new ArgumentNullException(nameof(jwtConfiguration));
    }

    public async Task<Guid> Handle(CreateUser request)
    {
        const string emailPattern =
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
        var isEmail = Regex.IsMatch(request.Email, emailPattern, RegexOptions.IgnoreCase);
        if (isEmail == false)
            throw new Exception("the email is not valid");

        var email = request.Email.ToUpperInvariant();
        await using var dbConnection = new NpgsqlConnection(_connectionString.Value);

        var fetchedEmail = await dbConnection.QueryFirstOrDefaultAsync<string>(
            "SELECT email FROM auth.user WHERE email = @email", new {email});

        if (!string.IsNullOrEmpty(fetchedEmail))
            throw new Exception("the user already exists");

        var userId = Guid.NewGuid();
        var (passwordHash, saltKey) = GetPasswordHash(request.Password);

        await dbConnection.ExecuteAsync(
            "INSERT INTO auth.user (id, email, password_hash, salt_key, claims) VALUES (@userId, @email, @passwordHash, @saltKey, @claims)",
            new {userId, email, passwordHash, saltKey, request.Claims});

        return userId;
    }

    public async Task<string> Handle(Login request)
    {
        await using var dbConnection = new NpgsqlConnection(_connectionString.Value);

        var email = request.Email.ToUpperInvariant();
        var user = await dbConnection.QueryFirstOrDefaultAsync<UserDatabase>(
            @"SELECT
                id AS Id,
                claims AS Claims,
                salt_key AS SaltKey,
                password_hash AS PasswordHash
              FROM auth.user where email = @email", new {email});

        if (user is null)
            throw new Exception("user does not exists");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password, user.SaltKey);
        if (hash != user.PasswordHash)
            throw new Exception("invalid password");

        var credentials = new SigningCredentials(_jwtConfiguration.SigningKey, SecurityAlgorithms.HmacSha512);
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, request.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }.Concat(user.Claims.Cast<string>().Select(e => new Claim(ClaimTypes.Role, e)))),
            Expires = DateTime.Now.AddMinutes(5),
            Audience = _jwtConfiguration.Audience,
            Issuer = _jwtConfiguration.Issuer,
            SigningCredentials = credentials
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }

    private static (string hash, string saltKey) GetPasswordHash(string password)
    {
        var saltKey = BCrypt.Net.BCrypt.GenerateSalt();
        var hash = BCrypt.Net.BCrypt.HashPassword(password, saltKey);
        return (hash, saltKey);
    }
}

internal record UserDatabase(Guid Id, Array Claims, string SaltKey, string PasswordHash);