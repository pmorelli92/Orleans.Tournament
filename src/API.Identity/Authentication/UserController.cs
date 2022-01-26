using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans.Tournament.Utils.Mvc.Responses;

namespace Orleans.Tournament.API.Identity.Authentication
{
    public class UserController : ControllerBase
    {
        private readonly IUserStore _userStore;

        public UserController(IUserStore userStore)
        {
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
        }

        [HttpPost("api/user/create", Name = "Create user")]
        [ProducesResponseType(typeof(ResourceResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateTeam([FromBody] CreateUser model)
        {
            var claims = new[] { "write", "read" };
            var userResult = await _userStore.CreateUserAsync(model.Email, model.Password, model.Claims);

            return userResult.Match<IActionResult>(
                s => Created(s.ToString(), new { id = s }),
                f => Conflict());
        }
    }

    public class CreateUser
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public IList<string> Claims { get; set; }
    }
}