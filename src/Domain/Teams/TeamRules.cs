using LanguageExt;
using Snaelro.Domain.Teams.ValueObjects;

namespace Snaelro.Domain.Teams
{
    public static class TeamRules
    {
        public static Validation<TeamErrorCodes, State> TeamExists(State state)
        {
            if (state.Created)
                return state;

            return TeamErrorCodes.TeamDoesNotExist;
        }
    }

    public enum TeamErrorCodes
    {
        TeamDoesNotExist = 1
    }
}