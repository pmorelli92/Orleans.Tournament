namespace Snaelro.Domain.Teams.Events
{
    public class PlayerAdded
    {
        public string Name { get; }

        public PlayerAdded(string name)
        {
            Name = name;
        }
    }
}