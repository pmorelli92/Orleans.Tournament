using LanguageExt;

namespace Snaelro.Domain.Teams
{
    public static class TeamRules
    {
        public static Validation<TeamErrorCodes, Unit> TeamExists(TeamState state)
        {
            if (state.Created)
                return Unit.Default;

            return TeamErrorCodes.TeamDoesNotExist;
        }
    }

    public enum TeamErrorCodes
    {
        TeamDoesNotExist = 1
    }
}