using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Tournament.Domain.Tournaments;
using Tournament.Projections.Tournaments;

namespace Tournament.API.Tournaments;

public class Controller : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ITournamentQueryHandler _tournamentQueryHandler;

    public Controller(
        IClusterClient clusterClient,
        ITournamentQueryHandler tournamentQueryHandler)
    {
        _clusterClient = clusterClient;
        _tournamentQueryHandler = tournamentQueryHandler;
    }

    private Guid GetUserId
        => new Guid(User.Claims.Single(e => e.Type == ClaimTypes.NameIdentifier).Value);

    [Authorize(Roles = "write")]
    [HttpPost("api/tournament/create", Name = "Create tournament")]
    [ProducesResponseType(typeof(ResourceResponse), (int)HttpStatusCode.OK)]
    public IActionResult CreateTournament([FromBody] CreateTournamentModel model)
    {
        var tournamentId = Guid.NewGuid();
        var traceId = Guid.NewGuid();
        var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

        tournament.CreateAsync(new CreateTournament(model.Name, tournamentId, traceId, GetUserId));

        return Created(tournamentId.ToString(), new ResourceResponse(tournamentId, traceId));
    }

    [Authorize(Roles = "write")]
    [HttpPut("api/tournament/{tournamentId:Guid}/team", Name = "Add team to tournament")]
    [ProducesResponseType(typeof(TraceResponse), (int)HttpStatusCode.OK)]
    public IActionResult AddTeams([FromRoute] Guid tournamentId, [FromBody] AddTeamModel model)
    {
        var traceId = Guid.NewGuid();
        var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

        tournament.AddTeamAsync(new AddTeam(tournamentId, model.TeamId, traceId, GetUserId));
        return Ok(new TraceResponse(traceId));
    }

    [Authorize(Roles = "write")]
    [HttpPut("api/tournament/{tournamentId:Guid}/start", Name = "Start tournament")]
    [ProducesResponseType(typeof(TraceResponse), (int)HttpStatusCode.OK)]
    public IActionResult StartTournament([FromRoute] Guid tournamentId)
    {
        var traceId = Guid.NewGuid();
        var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

        tournament.StartAsync(new StartTournament(tournamentId, traceId, GetUserId));
        return Ok(new TraceResponse(traceId));
    }

    [Authorize(Roles = "write")]
    [HttpPut("api/tournament/{tournamentId:Guid}/setMatchResult", Name = "Set Match Result")]
    [ProducesResponseType(typeof(TraceResponse), (int)HttpStatusCode.OK)]
    public IActionResult SetMatchResult([FromRoute] Guid tournamentId, [FromBody] SetMatchResultModel model)
    {
        var traceId = Guid.NewGuid();
        var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

        var matchInfo = new Match(model.LocalTeamId, model.AwayTeamId, new MatchResult(model.LocalGoals, model.AwayGoals, true));

        tournament.SetMatchResultAsync(new SetMatchResult(tournamentId, matchInfo, traceId, GetUserId));
        return Ok(new TraceResponse(traceId));
    }

    [Authorize(Roles = "read")]
    [HttpGet("api/tournament/{tournamentId:Guid}", Name = "Get tournament")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(TournamentResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTournament([FromRoute] Guid tournamentId)
    {
        try
        {
            var projection = await _tournamentQueryHandler.GetTournamentAsync(tournamentId);
            return Ok(TournamentResponse.From(projection));
        }
        catch
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "read")]
    [HttpGet("api/tournaments", Name = "Get tournaments")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(TournamentResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetTournaments()
    {
        var projection = await _tournamentQueryHandler.GetTournamentsAsync();
        return Ok(TournamentResponse.From(projection));
    }
}
