using LanguageExt;

namespace Snaelro.Domain.Tournaments
{
    public static class TournamentRules
    {
        public static Validation<TournamentErrorCodes, TournamentState> TournamentExists(TournamentState state)
        {
            if (state.Created)
                return state;

            return TournamentErrorCodes.TournamentDoesNotExist;
        }
    }

    public enum TournamentErrorCodes
    {
        TournamentDoesNotExist = 1
    }
}