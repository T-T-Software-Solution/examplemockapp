using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain
{
    public interface IBaseEntity<Key>
    {
        Key id { get; set; }

        DateTime? created { get; set; }

        DateTime? updated { get; set; }

        bool? isActive { get; set; }

    }
}
