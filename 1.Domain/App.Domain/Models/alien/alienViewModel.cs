using System.Collections.Generic;

namespace App.Domain
{
    public class alienViewModel : BaseViewModel<Guid>
    {

        public string? name { get; set; }

        public string? species { get; set; }

        public string? origin_planet { get; set; }


    }
}