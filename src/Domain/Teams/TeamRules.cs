using LanguageExt;

namespace Snaelro.Domain.Teams
{
    public static class TeamRules
    {
        public static Validation<TeamErrorCodes, TeamState> TeamExists(TeamState state)
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