namespace Snaelro.Domain.Teams.Commands
{
    public class AddPlayer
    {
        public string Name { get; }

        public AddPlayer(string name)
        {
            Name = name;
        }
    }
}