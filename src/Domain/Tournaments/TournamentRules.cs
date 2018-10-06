using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using LanguageExt;
using Orleans;
using Snaelro.Domain.Teams;
using Snaelro.Domain.Tournaments.ValueObject;

namespace Snaelro.Domain.Tournaments
{
    public static class TournamentRules
    {
        public static Validation<TournamentErrorCodes, Unit> TournamentExists(TournamentState state)
        {
            if (state.Created) return Unit.Default;

            return TournamentErrorCodes.TournamentDoesNotExist;
        }

        public static Validation<TournamentErrorCodes, Unit> TeamExistsForward(bool exist)
        {
            if (exist) return Unit.Default;

            return TournamentErrorCodes.TeamDoesNotExist;
        }

        public static Validation<TournamentErrorCodes, Unit> TeamIsNotAdded(TournamentState state, Guid teamId)
        {
            if (state.Teams.Any(e => e == teamId)) return TournamentErrorCodes.TeamIsAlreadyAdded;

            return Unit.Default;
        }

        public static Validation<TournamentErrorCodes, Unit> LessThanEightTeams(TournamentState state)
        {
            if (state.Teams.Count < 8) return Unit.Default;

            return TournamentErrorCodes.TournamentHasMoreThanEightTeams;
        }

        public static Validation<TournamentErrorCodes, Unit> EightTeamsToStartTournament(TournamentState state)
        {
            if (state.Teams.Count == 8) return Unit.Default;

            return TournamentErrorCodes.CantStartTournamentWithLessThanEightTeams;
        }

        public static Validation<TournamentErrorCodes, Unit> TournamentIsStarted(TournamentState state)
        {
            if (state.Fixture != null) return Unit.Default;

            return TournamentErrorCodes.TournamentIsNotStarted;
        }

        public static Validation<TournamentErrorCodes, Unit> TournamentIsNotStarted(TournamentState state)
        {
            if (state.Fixture is null) return Unit.Default;

            return TournamentErrorCodes.TournamentIsStarted;
        }

        public static Validation<TournamentErrorCodes, Unit> TournamentIsNotOnFinals(TournamentState state)
        {
            if (state.Fixture.FinalPhase == false) return Unit.Default;

            return TournamentErrorCodes.TournamentAlreadyOnFinals;
        }

        public static Validation<TournamentErrorCodes, Unit> TournamentMatchExists(TournamentState state, MatchResult matchResult)
        {
            // Last bracket's matches list
            var bracketMatches = state.Fixture.Brackets.Last().Value;

            // Find the match
            var match = bracketMatches.SingleOrDefault(e => e.ResultRelatesToMatch(matchResult));

            if (match is null) return TournamentErrorCodes.MatchDoesNotExist;

            if (match.Played) return TournamentErrorCodes.MatchAlreadyPlayed;

            return Unit.Default;
        }

        public static Validation<TournamentErrorCodes, Unit> MatchIsNotDraw(MatchResult matchResult)
        {
            if (matchResult.LocalGoals == matchResult.AwayGoals) return TournamentErrorCodes.DrawResultIsNotAllowed;

            return Unit.Default;
        }

        public static Validation<TournamentErrorCodes, Unit> TournamentPhaseCompleted(TournamentState state)
        {
            // Last bracket's matches list
            var bracketMatches = state.Fixture.Brackets.Last().Value;

            // Find the match
            if (bracketMatches.All(e => e.Played)) return Unit.Default;

            return TournamentErrorCodes.NotAllMatchesPlayed;
        }
    }

    public enum TournamentErrorCodes
    {
        TournamentDoesNotExist = 1,
        TournamentHasMoreThanEightTeams = 2,
        CantStartTournamentWithLessThanEightTeams = 3,
        TournamentIsNotStarted = 4,
        MatchDoesNotExist = 5,
        MatchAlreadyPlayed = 6,
        NotAllMatchesPlayed = 7,
        DrawResultIsNotAllowed = 8,
        TournamentAlreadyOnFinals = 9,
        TournamentIsStarted = 10,
        TeamDoesNotExist = 11,
        TeamIsAlreadyAdded = 12
    }
}