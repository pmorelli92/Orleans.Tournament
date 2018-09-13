using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Snaelro.API.Teams.Input;
using Snaelro.Domain.Teams.Aggregates;
using Snaelro.Domain.Teams.Commands;

namespace Snaelro.API.Teams.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public TeamController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpPut("api/team/create/{teamName}", Name = "Create team")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> CreateTeam([FromRoute] string teamName)
        {
            var teamId = Guid.NewGuid();
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

            await team.CreateAsync(new CreateTeam(teamName));

            return Ok(new { id = teamId });
        }

        [HttpPut("api/team/{teamId:Guid}/player", Name = "Add player to team")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> AddPlayers([FromRoute] Guid teamId, [FromBody] PlayerModel model)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

            await Task.WhenAll(model.Names
                .Select(e => new AddPlayer(e))
                .Select(async e => await team.AddPlayerAsync(e)));

            return Ok(new { id = teamId });
        }

        [HttpGet("api/team/{teamId:Guid}", Name = "Get team name")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTeamName([FromRoute] Guid teamId)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var teamName = await team.GetNameAsync();
            return Ok(teamName);
        }

        [HttpGet("api/team/{teamId:Guid}/players", Name = "Get team players")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTeamPlayers([FromRoute] Guid teamId)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var teamName = await team.GetPlayersAsync();
            return Ok(teamName);
        }
    }
}