using System.Collections.Generic;

namespace Snaelro.API.Teams.Input
{
    public class CreateModel
    {
        public string Name { get; set; }
    }

    public class PlayerModel
    {
        public List<string> Names { get; set; }
    }
}