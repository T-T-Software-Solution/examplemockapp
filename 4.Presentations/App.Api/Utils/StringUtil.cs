using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TTSW.Constant;
using MimeTypes;
using System.Net.Http;
using System.Xml.Serialization;
using System.Xml;

namespace TTSW.Utils
{
    public class StringUtil
    {
        public StringUtil()
        {

        }

        public static string DeleteLines(string input, int linesToSkip)
        {
            int startIndex = 0;
            for (int i = 0; i < linesToSkip; ++i)
                startIndex = input.IndexOf('\n', startIndex) + 1;
            return input.Substring(startIndex);
        }

        public static string RemoveLastLine(string str)
        {
            return str.Remove(str.TrimEnd().LastIndexOf(Environment.NewLine));
        }
    }
}
