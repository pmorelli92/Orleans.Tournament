using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Tournament.Domain.Teams;
using Tournament.Projections.Teams;

namespace Tournament.API.Teams;

public class Controller : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ITeamQueryHandler _teamQueryHandler;

    public Controller(
        IClusterClient clusterClient,
        ITeamQueryHandler teamQueryHandler)
    {
        _clusterClient = clusterClient;
        _teamQueryHandler = teamQueryHandler;
    }

    private Guid GetUserId
        => new Guid(User.Claims.Single(e => e.Type == ClaimTypes.NameIdentifier).Value);

    [Authorize(Roles = "write")]
    [HttpPost("api/team/create", Name = "Create team")]
    [ProducesResponseType(typeof(ResourceResponse), (int)HttpStatusCode.OK)]
    public IActionResult CreateTeam([FromBody] CreateTeamModel model)
    {
        var teamId = Guid.NewGuid();
        var traceId = Guid.NewGuid();
        var team = _clusterClient.GetGrain<ITeamGrain>(teamId);

        team.CreateAsync(new CreateTeam(model.Name, teamId, traceId, GetUserId));

        return Created(teamId.ToString(), new ResourceResponse(teamId, traceId));
    }

    [Authorize(Roles = "write")]
    [HttpPut("api/team/{teamId:Guid}/players", Name = "Add player to team")]
    [ProducesResponseType(typeof(TraceResponse), (int)HttpStatusCode.OK)]
    public IActionResult AddPlayers([FromRoute] Guid teamId, [FromBody] AddPlayerModel model)
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(TeamResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTeam([FromRoute] Guid teamId)
    {
        try
        {
            var projection = await _teamQueryHandler.GetTeamAsync(teamId);
            return Ok(TeamResponse.From(projection));
        }
        catch
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "read")]
    [HttpGet("api/teams", Name = "Get teams")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(TeamResponse[]), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTeams()
    {
        var projection = await _teamQueryHandler.GetTeamsAsync();
        return Ok(TeamResponse.From(projection));
    }
}
