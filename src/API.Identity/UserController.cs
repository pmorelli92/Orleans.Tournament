using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orleans.Tournament.API.Identity;

public class UserController : ControllerBase
{
    private readonly ICreateUser _createUser;
    private readonly ILogger<UserController> _logger;
    private readonly ILoginUser _loginUser;

    public UserController(ICreateUser createUser, ILoginUser loginUser, ILogger<UserController> logger)
    {
        _createUser = createUser ?? throw new ArgumentNullException(nameof(createUser));
        _loginUser = loginUser ?? throw new ArgumentNullException(nameof(loginUser));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [AllowAnonymous]
    [HttpPost("api/user/create", Name = "Create user")]
    [ProducesResponseType(typeof(UserResponse), (int) HttpStatusCode.Created)]
    public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
    {
        try
        {
            var (email, password, claims) = request;
            var userId = await _createUser.Handle(new CreateUser(email, password, claims));
            return Created($"api/user/{userId}", new UserResponse(userId));
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error creating user: {Message}", e.Message);
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpPost("api/user/login", Name = "Login user")]
    [ProducesResponseType(typeof(LoginResponse), (int) HttpStatusCode.OK)]
    public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
    {
        try
        {
            var (email, password) = request;
            var token = await _loginUser.Handle(new Login(email, password));
            return Ok(new LoginResponse(token));
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error login user: {Message}", e.Message);
            return BadRequest();
        }
    }
}

public readonly record struct UserRequest(string Email, string Password, IList<string> Claims);

public readonly record struct UserResponse(Guid UserId);

public readonly record struct LoginRequest(string Email, string Password);

public readonly record struct LoginResponse(string Token);