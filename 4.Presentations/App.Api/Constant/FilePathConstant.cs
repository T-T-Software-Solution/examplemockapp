using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTSW.Utils;

namespace TTSW.Constant {
    public class FilePathConstant {
        public enum DirType {
            [StringValue ("Uploads")]
            TempUpload,

            [StringValue ("Files")]
            Files,

            [StringValue ("Files")]
            FilesTestUpload,
            
        }
    }
}
