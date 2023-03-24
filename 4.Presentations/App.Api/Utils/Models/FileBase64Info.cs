using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Utils
{
    public class FileBase64Info
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public string MimeType { get; set; }

        public string DataURL
        {
            get
            {
                return string.Format("data:{0};base64,{1}", MimeType, Content);
            }
        }
    }
}
