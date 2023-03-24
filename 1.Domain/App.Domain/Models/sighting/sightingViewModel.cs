using System.Collections.Generic;

namespace App.Domain
{
    public class sightingViewModel : BaseViewModel<Guid>
    {

        public Guid? alien_id { get; set; }

        public DateTime? found_date { get; set; }

        public string txt_found_date { get { return Common.GetDateStringForReport(this.found_date); } }

        public string? location { get; set; }

        public string? witness { get; set; }

        public string? alien_id_alien_species { get; set; }

    }
}