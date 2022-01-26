using System.Collections.Generic;

namespace Orleans.Tournament.API.Teams.Input
{
    public class AddPlayerModel
    {
        public IEnumerable<string> Names { get; set; }
    }
}