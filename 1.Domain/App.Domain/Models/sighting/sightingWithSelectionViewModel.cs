using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain
{
    public class sightingWithSelectionViewModel: sightingViewModel
    {
        public List<alienViewModel>? item_alien_id { get; set; }

    }
}