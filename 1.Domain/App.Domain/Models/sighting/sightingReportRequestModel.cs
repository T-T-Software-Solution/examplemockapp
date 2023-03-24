using System.Collections.Generic;

namespace App.Domain
{
    public class sightingReportRequestModel : sightingSearchModel
    {
	    public string filetype { get; set; }

        public string contentType { get { return Common.GetContentType(filetype); } }
    }
}

