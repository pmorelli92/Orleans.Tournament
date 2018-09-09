namespace Snaelro.Domain.Teams.Events
{
    public class Echoed
    {
        public string Message { get; }

        public Echoed(string message)
        {
            Message = message;
        }
    }
}