using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Utils
{
    public class CommonResponseMessage
    {
        public string? code { get; set; }
        public string? message { get; set; }
        public object? data { get; set; }
    }
}
