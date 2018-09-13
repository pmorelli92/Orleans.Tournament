namespace Snaelro.Domain.Teams.Commands
{
    public class CreateTeam
    {
        public string Name { get; }

        public CreateTeam(string name)
        {
            Name = name;
        }
    }
}