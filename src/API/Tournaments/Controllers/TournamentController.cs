using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Snaelro.API.Tournaments.Input;
using Snaelro.Domain.Tournaments;
using Snaelro.Domain.Tournaments.Commands;
using Snaelro.Domain.Tournaments.ValueObject;
using Snaelro.Utils.Mvc.Responses;

namespace Snaelro.API.Tournaments.Controllers
{
    public class TournamentController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public TournamentController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        private Guid GetUserId
            => new Guid(User.Claims.Single(e => e.Type == ClaimTypes.NameIdentifier).Value);

        [Authorize(Roles = "write")]
        [HttpPost("api/tournament/create", Name = "Create tournament")]
        [ProducesResponseType(typeof(ResourceResponse), (int) HttpStatusCode.OK)]
        public IActionResult CreateTournamnent([FromBody] CreateTournamentModel model)
        {
            var tournamentId = Guid.NewGuid();
            var traceId = Guid.NewGuid();
            var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

            tournament.CreateAsync(new CreateTournament(model.Name, tournamentId, traceId, GetUserId));

            return Created(tournamentId.ToString(), new ResourceResponse(tournamentId, traceId));
        }

        [Authorize(Roles = "write")]
        [HttpPut("api/tournament/{tournamentId:Guid}/team", Name = "Add team to tournament")]
        [ProducesResponseType(typeof(TraceResponse), (int) HttpStatusCode.OK)]
        public IActionResult AddTeams([FromRoute] Guid tournamentId, [FromBody] AddTeamModel model)
        {
            var traceId = Guid.NewGuid();
            var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

            tournament.AddTeamAsync(new AddTeam(tournamentId, model.TeamId, traceId, GetUserId));
            return Ok(new TraceResponse(traceId));
        }

        [Authorize(Roles = "write")]
        [HttpPut("api/tournament/{tournamentId:Guid}/start", Name = "Start tournament")]
        [ProducesResponseType(typeof(TraceResponse), (int) HttpStatusCode.OK)]
        public IActionResult StartTournament([FromRoute] Guid tournamentId)
        {
            var traceId = Guid.NewGuid();
            var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

            tournament.StartAsync(new StartTournament(tournamentId, traceId, GetUserId));
            return Ok(new TraceResponse(traceId));
        }

        [Authorize(Roles = "write")]
        [HttpPut("api/tournament/{tournamentId:Guid}/setMatchResult", Name = "Set Match Result")]
        [ProducesResponseType(typeof(TraceResponse), (int) HttpStatusCode.OK)]
        public IActionResult SetMatchResult([FromRoute] Guid tournamentId, [FromBody] SetMatchResultModel model)
        {
            var traceId = Guid.NewGuid();
            var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

            var matchResult = new MatchResult(model.LocalTeamId, model.LocalGoals, model.AwayTeamId, model.AwayGoals);

            tournament.SetMatchResultAsync(new SetMatchResult(tournamentId, matchResult, traceId, GetUserId));
            return Ok(new TraceResponse(traceId));
        }

        [Authorize(Roles = "write")]
        [HttpPut("api/tournament/{tournamentId:Guid}/nextPhase", Name = "Start next phase")]
        [ProducesResponseType(typeof(TraceResponse), (int) HttpStatusCode.OK)]
        public IActionResult StartNextPhase([FromRoute] Guid tournamentId)
        {
            var traceId = Guid.NewGuid();
            var tournament = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);

            tournament.NextPhaseAsync(new StartNextPhase(tournamentId, traceId, GetUserId));
            return Ok(new TraceResponse(traceId));
        }

        [Authorize(Roles = "read")]
        [HttpGet("api/tournament/{tournamentId:Guid}", Name = "Get tournament")]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(TournamentState), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTournament([FromRoute] Guid tournamentId)
        {
            var team = _clusterClient.GetGrain<ITournamentGrain>(tournamentId);
            var tournamentResult = await team.GetTournamentAsync();

            return tournamentResult.Match<IActionResult>(
                s => Ok(s),
                f => NotFound());
        }
    }
}