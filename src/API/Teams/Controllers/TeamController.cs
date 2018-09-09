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

        [HttpPut("api/team/echo/{teamId:Guid}/{message}", Name = "Team echo")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Echo([FromRoute] Guid teamId, [FromRoute] string message)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var echo = await team.EchoAsync(message);
            return Ok(echo);
        }

        [HttpGet("api/team/echo/{teamId:Guid}", Name = "Team messages")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetMessages([FromRoute] Guid teamId)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var messages = await team.GetMessagesAsync();
            return Ok(messages);
        }
    }
}