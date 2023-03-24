using System.Collections.Generic;

namespace App.Domain
{
    public class sightingInputModel
    {

        public Guid? id { get; set; }

        public Guid? alien_id { get; set; }

        public DateTime? found_date { get; set; }

        public string? location { get; set; }

        public string? witness { get; set; }

        public string? active_mode { get; set; }
    }
}

