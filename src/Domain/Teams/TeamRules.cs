using LanguageExt;

namespace Orleans.Tournament.Domain.Teams
{
    public static class TeamRules
    {
        public static Validation<BusinessErrors, Unit> TeamExists(TeamState state)
        {
            if (state.Created)
                return Unit.Default;

            return BusinessErrors.TeamDoesNotExist;
        }

        public static Validation<BusinessErrors, Unit> TeamDoesNotExists(TeamState state)
        {
            if (state.Created)
                return BusinessErrors.TeamAlreadyExist;

            return Unit.Default;
        }
    }
}
