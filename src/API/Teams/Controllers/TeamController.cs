using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Snaelro.Domain.Teams.Aggregates;

namespace Snaelro.API.Teams.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public TeamController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet("api/team/echo/{message}", Name = "Team echo")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> HelloWorld([FromRoute] string message)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(Guid.NewGuid());
            var echo = await team.EchoAsync(message);
            return Ok(echo);
        }
    }
}