using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Snaelro.API.Teams.Input;
using Snaelro.Domain.Teams;
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
        public IActionResult CreateTeam([FromRoute] string teamName)
        {
            var teamId = Guid.NewGuid();
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

            team.CreateAsync(new CreateTeam(teamName));

            return Ok(new { id = teamId });
        }

        [HttpPut("api/team/{teamId:Guid}/players", Name = "Add player to team")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public IActionResult AddPlayers([FromRoute] Guid teamId, [FromBody] PlayerModel model)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

            model.Names
                .Select(e => new AddPlayer(e))
                .ToList()
                .ForEach(e => team.AddPlayerAsync(e));

            return Ok(new { id = teamId });
        }

        [HttpGet("api/team/{teamId:Guid}", Name = "Get team name")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTeamName([FromRoute] Guid teamId)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var nameResult = await team.GetNameAsync();

            return nameResult.Match<IActionResult>(Ok, f => BadRequest());
        }

        [HttpGet("api/team/{teamId:Guid}/players", Name = "Get team players")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTeamPlayers([FromRoute] Guid teamId)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var playersResult = await team.GetPlayersAsync();

            return playersResult.Match<IActionResult>(Ok, f => BadRequest());
        }
    }
}