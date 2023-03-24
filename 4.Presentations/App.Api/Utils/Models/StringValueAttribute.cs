using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Utils
{
    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

    }
}
