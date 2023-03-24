using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Utils
{
    public class KeyPairWithParentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
    }
}
