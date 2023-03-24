using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Domain
{
    public class alienEntity : BaseEntity<Guid>
    {


        [MaxLength(4000), Column(Order = 2), Comment("Alien Name")]
        public string? name { get; set; }

        [MaxLength(4000), Column(Order = 3), Comment("Species")]
        public string? species { get; set; }

        [MaxLength(4000), Column(Order = 4), Comment("Origin Planet")]
        public string? origin_planet { get; set; }


        public List<sightingEntity> sightings { get; } = new();


    }
}
