namespace Snaelro.Domain.Teams.Events
{
    public class TeamCreated
    {
        public string Name { get; }

        public TeamCreated(string name)
        {
            Name = name;
        }
    }
}