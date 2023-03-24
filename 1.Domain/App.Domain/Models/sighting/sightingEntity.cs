using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Domain
{
    public class sightingEntity : BaseEntity<Guid>
    {


        [ForeignKey("alien_id")]
        public alienEntity? alien_alien_id { get; set; }

        [Column(Order = 2), Comment("Alien")]
        public Guid? alien_id { get; set; }

        [Column(Order = 3), Comment("Found Date")]
        public DateTime? found_date { get; set; }

        [Column(Order = 4), Comment("Location")]
        public string? location { get; set; }

        [Column(Order = 5), Comment("Witness")]
        public string? witness { get; set; }




    }
}
