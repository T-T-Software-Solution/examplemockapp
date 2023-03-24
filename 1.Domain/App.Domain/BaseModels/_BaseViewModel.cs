using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain
{
    public class BaseViewModel<Key> : IBaseViewModel<Key>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Key id { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime? created {get; set;}

        public DateTime? updated {get; set;}
        public bool? isActive { get; set;}
    }
}
