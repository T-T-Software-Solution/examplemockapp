using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Utils
{
	public class StaticConfigModel
	{
		#region Properties
		public Dictionary<string, FileInfo> Files { get; set; }
		public Dictionary<string, DirectoryInfo> Directories { get; set; }
		#endregion
	}


}
