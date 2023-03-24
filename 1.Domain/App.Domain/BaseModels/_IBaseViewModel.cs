using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain
{
    public interface IBaseViewModel<Key>
    {

        DateTime? created { get; set; }

        DateTime? updated { get; set; }
        
        bool? isActive { get; set; }

    }
}
