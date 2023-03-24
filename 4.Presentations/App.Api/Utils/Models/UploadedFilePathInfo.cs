using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Utils
{
    public class UploadedFilePathInfo
    {
        public int Count { get; set; }
        public long Size { get; set; }
        public List<string> FilesUploaded { get; set; }
    }
}
