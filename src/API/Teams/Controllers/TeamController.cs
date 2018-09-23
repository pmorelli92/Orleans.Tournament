using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Snaelro.API.Teams.Input;
using Snaelro.API.Teams.Output;
using Snaelro.Domain.Teams;
using Snaelro.Domain.Teams.Commands;
using Snaelro.Utils.Mvc.Responses;

namespace Snaelro.API.Teams.Controllers
{
    public class TeamController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public TeamController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        private Guid GetUserId
            => new Guid(User.Claims.Single(e => e.Type == ClaimTypes.NameIdentifier).Value);

        [Authorize(Roles = "write")]
        [HttpPost("api/team/create", Name = "Create team")]
        [ProducesResponseType(typeof(ResourceResponse), (int) HttpStatusCode.OK)]
        public IActionResult CreateTeam([FromBody] CreateModel model)
        {
            var teamId = Guid.NewGuid();
            var traceId = Guid.NewGuid();
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

            team.CreateAsync(new CreateTeam(model.Name, teamId, traceId, GetUserId));

            return Created(teamId.ToString(), new ResourceResponse(teamId, traceId));
        }

        [Authorize(Roles = "write")]
        [HttpPut("api/team/{teamId:Guid}/players", Name = "Add player to team")]
        [ProducesResponseType(typeof(TraceResponse), (int) HttpStatusCode.OK)]
        public IActionResult AddPlayers([FromRoute] Guid teamId, [FromBody] PlayerModel model)
        {
            var traceId = Guid.NewGuid();
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

            model.Names
                .Select(e => new AddPlayer(e, teamId, traceId, GetUserId))
                .ToList()
                .ForEach(e => team.AddPlayerAsync(e));

            return Ok(new TraceResponse(traceId));
        }

        [Authorize(Roles = "read")]
        [HttpGet("api/team/{teamId:Guid}", Name = "Get team")]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(TeamResponse), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTeam([FromRoute] Guid teamId)
        {
            var team = _clusterClient.GetGrain<ITeamGrain>(teamId);
            var teamResult = await team.GetTeamAsync();

            return teamResult.Match<IActionResult>(
                s => Ok(TeamResponse.From(s)),
                f => NotFound());
        }
    }
}