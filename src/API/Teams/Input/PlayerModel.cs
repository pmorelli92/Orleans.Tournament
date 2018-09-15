using System;
using System.Collections.Generic;

namespace Snaelro.API.Teams.Input
{
    public class CreateModel
    {
        public Guid TraceId { get; set; }

        public string Name { get; set; }
    }

    public class PlayerModel
    {
        public Guid TraceId { get; set; }

        public List<string> Names { get; set; }
    }
}