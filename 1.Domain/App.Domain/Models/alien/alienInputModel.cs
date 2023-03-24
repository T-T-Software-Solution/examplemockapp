using System.Collections.Generic;

namespace App.Domain
{
    public class alienInputModel
    {

        public Guid? id { get; set; }

        public string? name { get; set; }

        public string? species { get; set; }

        public string? origin_planet { get; set; }

        public string? active_mode { get; set; }
    }
}

