using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using LanguageExt;
using Orleans;
using Snaelro.Domain.Teams;
using Snaelro.Domain.Tournaments.ValueObject;

namespace Snaelro.Domain.Tournaments
{
    public static class TournamentRules
    {
        public static Validation<BusinessErrors, Unit> TournamentExists(TournamentState state)
        {
            if (state.Created) return Unit.Default;

            return BusinessErrors.TournamentDoesNotExist;
        }

        public static Validation<BusinessErrors, Unit> TournamentDoesNotExists(TournamentState state)
        {
            if (state.Created) return BusinessErrors.TournamentAlreadyExist;

            return Unit.Default;
        }

        public static Validation<BusinessErrors, Unit> TeamExistsForward(bool exist)
        {
            if (exist) return Unit.Default;

            return BusinessErrors.TeamDoesNotExist;
        }

        public static Validation<BusinessErrors, Unit> TeamIsNotAdded(TournamentState state, Guid teamId)
        {
            if (state.Teams.Any(e => e == teamId)) return BusinessErrors.TeamIsAlreadyAdded;

            return Unit.Default;
        }

        public static Validation<BusinessErrors, Unit> LessThanEightTeams(TournamentState state)
        {
            if (state.Teams.Count < 8) return Unit.Default;

            return BusinessErrors.TournamentHasMoreThanEightTeams;
        }

        public static Validation<BusinessErrors, Unit> EightTeamsToStartTournament(TournamentState state)
        {
            if (state.Teams.Count == 8) return Unit.Default;

            return BusinessErrors.CantStartTournamentWithLessThanEightTeams;
        }

        public static Validation<BusinessErrors, Unit> TournamentIsStarted(TournamentState state)
        {
            if (state.Fixture != null) return Unit.Default;

            return BusinessErrors.TournamentIsNotStarted;
        }

        public static Validation<BusinessErrors, Unit> TournamentIsNotStarted(TournamentState state)
        {
            if (state.Fixture is null) return Unit.Default;

            return BusinessErrors.TournamentAlreadyStarted;
        }

        public static Validation<BusinessErrors, Unit> TournamentIsNotOnFinals(TournamentState state)
        {
            if (state.Fixture.FinalPhase == false) return Unit.Default;

            return BusinessErrors.TournamentAlreadyOnFinals;
        }

        public static Validation<BusinessErrors, Unit> TournamentMatchExistsAndIsNotPlayed(TournamentState state, MatchResult matchResult)
        {
            if (state.Fixture is null) return BusinessErrors.MatchDoesNotExist;

            // Last bracket's matches list
            var bracketMatches = state.Fixture.Brackets.Last().Value;

            // Find the match
            var match = bracketMatches.SingleOrDefault(e => e.ResultRelatesToMatch(matchResult));

            if (match is null) return BusinessErrors.MatchDoesNotExist;

            if (match.MatchSummary != null) return BusinessErrors.MatchAlreadyPlayed;

            return Unit.Default;
        }

        public static Validation<BusinessErrors, Unit> MatchIsNotDraw(MatchResult matchResult)
        {
            if (matchResult.LocalGoals == matchResult.AwayGoals) return BusinessErrors.DrawResultIsNotAllowed;

            return Unit.Default;
        }

        public static Validation<BusinessErrors, Unit> TournamentPhaseCompleted(TournamentState state)
        {
            // Last bracket's matches list
            var bracketMatches = state.Fixture.Brackets.Last().Value;

            // All match should be played
            if (bracketMatches.All(e => e.MatchSummary != null)) return Unit.Default;

            return BusinessErrors.NotAllMatchesPlayed;
        }
    }
}