using System.Collections.Immutable;
using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using LanguageExt;
using Npgsql;

namespace Orleans.Tournament.API.Identity.Authentication
{
    public interface IUserStore
    {
        Task<Validation<Unit, Guid>> CreateUserAsync(string email, string password, IList<string> claims);

        Task<Validation<Unit, User>> GetUserAsync(string email, string password);
    }

    public class UserStore : IUserStore
    {
        private readonly string _dbConnection;

        public UserStore(string dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        public async Task<Validation<Unit, Guid>> CreateUserAsync(string email, string password, IList<string> claims)
        {
            var isEmail = Regex.IsMatch(email,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
            if (isEmail == false) return Unit.Default;

            email = email.ToUpperInvariant();

            using (var dbConnection = new NpgsqlConnection(_dbConnection))
            {
                var emailExist = await dbConnection.CheckIfEmailExistAsync(email);
                if (emailExist != null) return Unit.Default;

                var userId = Guid.NewGuid();
                var bCrypt = GetPasswordHash(password);
                await dbConnection.InsertUserAsync(userId, email, bCrypt.hash, bCrypt.saltKey, claims);

                return userId;
            }
        }

        public async Task<Validation<Unit, User>> GetUserAsync(string email, string password)
        {
            using (var dbConnection = new NpgsqlConnection(_dbConnection))
            {
                var userInfo = await dbConnection.GetUserInfo(email.ToUpperInvariant());
                if (userInfo is null) return Unit.Default;

                var hash = BCrypt.Net.BCrypt.HashPassword(password, userInfo.SaltKey);
                if (hash != userInfo.PasswordHash) return Unit.Default;

                return new User(userInfo.UserId, userInfo.Claims.ToImmutableList());
            }
        }

        private static (string hash, string saltKey) GetPasswordHash(string password)
        {
            var saltKey = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(password, saltKey);
            return (hash, saltKey);
        }
    }
}

internal static class UserStoreExtensions
{
    internal static async Task<string> CheckIfEmailExistAsync(this IDbConnection @this, string email)
    {
        return await @this.QueryFirstOrDefaultAsync<string>(
            "SELECT email FROM auth.user WHERE email = @email", new {email});
    }

    internal static async Task InsertUserAsync(this IDbConnection @this,
        Guid userId, string email, string passwordHash, string saltKey, IList<string> claims)
    {
        await @this.ExecuteAsync(
            "INSERT INTO auth.user (id, email, password_hash, salt_key, claims) VALUES (@userId, @email, @passwordHash, @saltKey, @claims)",
            new {userId, email, passwordHash, saltKey, claims});
    }

    internal static async Task<UserInfo> GetUserInfo(this IDbConnection @this, string email)
    {
        return await @this.QueryFirstOrDefaultAsync<UserInfo>(
            @"SELECT
                id AS UserId,
                claims AS Claims,
                salt_key AS SaltKey,
                password_hash AS PasswordHash
              FROM auth.user where email = @email", new {email});
    }
}

public class UserInfo
{
    public Guid UserId { get; set; }

    public IReadOnlyList<string> Claims { get; set; }

    public string SaltKey { get; set; }

    public string PasswordHash { get; set; }
}